﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.UIElements;

namespace Colorful.RenderGraph
{
    public class RGNodeView : Node
    {
        public List<Port> outputPorts;
        public List<Port> inputPorts;

//         void InitNodeInfo(Type passType/*,BRGNode attachNode = null*/)
//         {
//             if (attachNode == null)
//                 node = new BRGNode(baseRenderPass);
//             else
//             {
//                 node = attachNode;
//                 var nodeIns = Activator.CreateInstance(node.nodeType) as RenderPass.BaseRenderNode;
//                 var fieldInfos = RGReflectionUtil.GetPublicField(baseRenderPass);
// 
//                 foreach (var realPara in node.parameters)
//                 {
//                     foreach (var fieldInfo in fieldInfos.Item3)
//                     {
//                         if(fieldInfo.Name == realPara.name)
//                         {
//                             realPara.type = fieldInfo;
//                             break;
//                         }
//                     }
// 
//                 }
//             }
//                 
// 
//             node.NodeView = this;

//             {
//                 
//                 }
//             }
// 
// 
//             node.NodeView = this;
// 
//             userData = node;
//         }
        void InitView(IEdgeConnectorListener edgeConnectorListener)
        {
            title = userData.GetType().Name;
            outputPorts = new List<Port>();
            inputPorts = new List<Port>();

            

            foreach (var item in (userData as IPass).__inputs)
            {
                var itemPort = PortView.Create(true, edgeConnectorListener, item);
                if (item.FieldType == typeof(TextureHandle))
                    itemPort.portColor = Color.green;
                else if (item.FieldType == typeof(RendererListHandle))
                    itemPort.portColor = Color.red;
                else if (item.FieldType == typeof(ComputeBufferHandle))
                    itemPort.portColor = Color.blue;
                inputPorts.Add(itemPort);
                inputContainer.Add(itemPort);
            };

            foreach (var item in (userData as IPass).__outputs)
            {
                var itemPort = PortView.Create(false, edgeConnectorListener, item);
                if (item.FieldType == typeof(TextureHandle))
                    itemPort.portColor = Color.green;
                else if (item.FieldType == typeof(RendererListHandle))
                    itemPort.portColor = Color.red;
                else if (item.FieldType == typeof(ComputeBufferHandle))
                    itemPort.portColor = Color.blue;
                outputPorts.Add(itemPort);
                outputContainer.Add(itemPort);
            };

            void RegisterChange(FieldInfo parameter,object obj)
            {
                parameter.SetValue(userData,obj);
            }
            foreach (var param in (userData as IPass).__publicSeriParams)
            {
                if(param.GetCustomAttribute<HideInInspector>() != null)
                {
                    continue;
                }
                VisualElement ele = null;
                if (param.FieldType == typeof(bool))
                {
                    var field = new Toggle(param.Name);
                    field.value = (bool)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(int))
                {
                    IntegerField field = new IntegerField(param.Name);
                    field.value = (int)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(float))
                {
                    FloatField field = new FloatField(param.Name);
                    field.value = (float)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType.IsEnum)
                {
                    var field = new EnumField(param.Name, (Enum)param.GetValue(userData));
                    field.value = (Enum)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(Vector2))
                {
                    var field = new Vector2Field(param.Name);
                    field.value = (Vector2)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(Vector2Int))
                {
                    var field = new Vector2IntField(param.Name);
                    field.value = (Vector2Int)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(Vector3))
                {
                    var field = new Vector3Field(param.Name);
                    field.value = (Vector3)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(Vector3Int))
                {
                    var field = new Vector3IntField(param.Name);
                    field.value = (Vector3Int)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(Vector4))
                {
                    var field = new Vector4Field(param.Name);
                    field.value = (Vector4)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (param.FieldType == typeof(Color))
                {
                    var field = new ColorField(param.Name);
                    field.value = (Color)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                else if (typeof(UnityEngine.Object) == param.FieldType.BaseType)
                {
                    var field = new ObjectField(param.Name);
                    field.objectType = param.FieldType;
                    field.value = (UnityEngine.Object)param.GetValue(userData);
                    field.RegisterValueChangedCallback(e => {
                        RegisterChange(param, e.newValue);
                    });
                    ele = field;
                }
                Add(ele);
            }
            
        }
        public RGNodeView(IPass pass,IEdgeConnectorListener edgeConnectorListener/*, BRGNode attachNode = null*/)
        {
            userData = pass;
            InitView(edgeConnectorListener);
        }

        public override void SetPosition(Rect newPos)
        {
            (userData as IPass).__position = newPos;
            base.SetPosition(newPos);
        }
    }

    public class PortView : Port
    {
        protected PortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public static PortView Create(bool input,IEdgeConnectorListener edgeConnectorListener,FieldInfo fieldInfo)
        {
            var port = new PortView(Orientation.Horizontal, input ? Direction.Input : Direction.Output, input ? Capacity.Single : Capacity.Multi, fieldInfo.FieldType)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(edgeConnectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            /*port.userData = slot;*/
            port.portName = fieldInfo.Name;
            return port;
        }
    }

}
