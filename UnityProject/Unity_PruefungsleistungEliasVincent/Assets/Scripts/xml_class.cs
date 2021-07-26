using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static System.Environment;
//Version 1.1 Tested (rewritten decodeXML() --> attribute decode has to be tested)
public class XMLobject{
    private string identifier;
    private List<XMLobject> childs;
    private string payload;
    private List<List<string>> attributes; 
    public string Identifier{get{return identifier;}}
    public string Payload{get{return payload;}}
    public int ChildCount{get{return childs.Count;}}
    public XMLobject(){
        attributes=new List<List<string>>();
        childs=new List<XMLobject>();
        payload="";
    }
    public XMLobject(string _identifier){
        identifier=_identifier;
        attributes=new List<List<string>>();
        childs=new List<XMLobject>();
        payload="";
    }
    public XMLobject(string _identifier,string _payload){
        identifier=_identifier;
        attributes=new List<List<string>>();
        childs=new List<XMLobject>();
        payload=_payload;
    }
    public XMLobject(string _identifier, params XMLobject[] child){
        identifier=_identifier;
        attributes=new List<List<string>>();
        childs=new List<XMLobject>();
        foreach (XMLobject xo in child){
            childs.Add(xo);
        }
    }
    public string decodeXML(string _xml){
        //remove linebraks
        _xml=_xml.Replace(NewLine,"");
        attributes=new List<List<string>>();
        string beforeXML="";
        string afterXML="";
        string tag="";
        string innerXML="";
        //find first tag
        int start=_xml.IndexOf('<');
        int end=_xml.IndexOf('>');
        if(start>-1 && end>-1 && start<end){
            //valid
            tag=_xml.Substring(start,end+1);
            //Debug.Log("extracted Tag: "+tag);
            if(start>0){
                beforeXML=_xml.Substring(0,start);
            }
            //get tag identifier
            if(tag.IndexOf(' ')>-1){
                //has attributes
                string a_tag=tag;
                identifier=a_tag.Substring(start+1,a_tag.IndexOf(' ')); //get identifier before attributes
                //Debug.Log("Tag with attribute found. Identifier: "+identifier);
                while(a_tag.IndexOf('=')>-1){
                    List<string> pair = new List<string>();
                    string a_name=a_tag.Substring(a_tag.IndexOf(' ')+1,a_tag.IndexOf('=')-1);
                    string a_value=a_tag.Substring(a_tag.IndexOf('"')+1,a_tag.IndexOf('"',a_tag.IndexOf('"')+1));
                    a_tag=a_tag.Remove(a_tag.IndexOf(' '),a_name.Length+a_value.Length+3);//remove attribute and value from tag
                    pair.Add(a_name);
                    pair.Add(a_value);
                    //Debug.Log("Found attribute: "+a_name+" with value: "+a_value);
                    attributes.Add(pair);
                }
                //Debug.Log("Tag after attribute extraction: "+a_tag);
            }
            else{
                //no attributes
                identifier=tag.Substring(start+1,tag.IndexOf('>')-1);
                //Debug.Log("Tag without attribute found. Identifier: "+identifier);
            }
            //get beforeXML
            if(_xml.IndexOf(tag)>0){
                beforeXML=_xml.Substring(0,_xml.IndexOf(tag)-1);
            }
            //get innerXML
            start=_xml.IndexOf(tag)+tag.Length;
            innerXML=_xml.Substring(start);
            Debug.Log("Extracted innerXML 1: "+innerXML);
            string endTag="</"+identifier+">";
            Debug.Log("Expected endTag: "+endTag);
            Debug.Log("Index of EndTag: "+innerXML.IndexOf(endTag).ToString());
            innerXML=innerXML.Remove(innerXML.IndexOf(endTag));
            //Debug.Log("Extracted innerXML 2: "+innerXML);
            //get afterXML
            start=_xml.IndexOf(endTag)+endTag.Length;
            afterXML=_xml.Substring(start);
            //Debug.Log("Extracted afterXML: "+afterXML);
            payload=innerXML;
            //test if innerXML contains XML
            while(isXML(innerXML)){
                //payload="";
                XMLobject xo=new XMLobject();
                innerXML=xo.decodeXML(innerXML);
                payload=innerXML;
                childs.Add(xo);
            }
            //Debug.Log("Object: "+identifier+" Payload: "+payload);
            return afterXML;
        }
        else{
            //invalid xml
            return _xml;
        }
    }
    public void addAttribute(string _identifier, string _value){
        List<string> a=new List<string>();
        a.Add(_identifier);
        a.Add(_value);
        attributes.Add(a);
    }
    public void addPayload(string _payload){
        payload+=_payload;
    }
    public void addChild(XMLobject child){
        childs.Add(child);
    }
    public XMLobject this[int index]
    {
        get{  
            if(index<childs.Count){
                return childs[index];
            }
            else{
                return null;
            }
        }
    }
    public XMLobject find(string _identifier){
        XMLobject erg=null;
        if(identifier==_identifier){
            return this;
        }
        if(childs.Count>0){
            foreach(XMLobject xo in childs){
                if(xo.Identifier==_identifier){
                    return xo;
                }
                else{
                    erg=xo.find(_identifier);
                    if(erg!=null){
                        return erg;
                    }
                }
            }
            return erg;
        }
        else{
            return null;
        }
    }
    public XMLobject findOnFirstLevel(string _identifier){
        XMLobject erg=null;
        if(identifier==_identifier){
            return this;
        }
        if(childs.Count>0){
            foreach(XMLobject xo in childs){
                if(xo.Identifier==_identifier){
                    return xo;
                }
            }
            return erg;
        }
        else{
            return null;
        }
    }
    
    public string serialize(){
        string output="<"+identifier;
        foreach(List<string> pair in attributes){
            output+=" "+pair[0]+"='"+pair[1]+"'";
        }
        output+=">";
        foreach(XMLobject xo in childs){
            output+=xo.serialize();
        }
        output+=payload+"</"+identifier+">"+NewLine;
        return output;
    }
    public bool isXML(string _xml){
        int start=_xml.IndexOf('<');
        int end=_xml.IndexOf('>');
        return (start>-1 && end>-1 && start<end);
    }
    public string getAttribute(string _identifier){
        foreach(List<string> attribute in attributes){
            if(attribute[0]==_identifier){
                return attribute[1];
            }
        }
        return "";
    }
}