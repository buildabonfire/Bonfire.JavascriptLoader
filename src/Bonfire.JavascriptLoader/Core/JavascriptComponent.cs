﻿using System;
using System.Web;
using System.Web.Helpers;

namespace Bonfire.JavascriptLoader.Core
{
    public class JavascriptComponent : IJavascriptComponent
    {
        private readonly IJavascriptEnvironment _environment;
        private object _props;
        private string _stringifiedProps;
        public string ComponentName { get; set; }
        public string ContainerId { get; set; }
        public string ContainerClass { get; set; }
        public string ContainerTag { get; set; }
        public bool RenderServerSide { get; set; }
        public object Props
        {
            get { return _props; }
            set
            {
                _props = value;
                _stringifiedProps = Json.Encode(value);
            }
        }

        public JavascriptComponent(IJavascriptEnvironment environment, string name, object props, string id, bool renderServerSide)
        {
            _environment = environment;
            ComponentName = name;
            ContainerId = string.IsNullOrEmpty(id) ? GenerateId() : id;
            ContainerTag = "div";
            RenderServerSide = renderServerSide;
            Props = props;
        }

        public string RenderHtml(string serverGlobal)
        {
            var attributes = string.Format("id=\"{0}\"", ContainerId);
            var html = "";

            if (!string.IsNullOrEmpty(ContainerClass))
            {
                attributes += string.Format(" class=\"{0}\"", ContainerClass);
            }

            if (RenderServerSide)
            {
                var script = string.Format(
                    "{2}.render('{0}', {1})",
                    ComponentName,
                    new HtmlString(_stringifiedProps),
                    serverGlobal
                );

                try
                {
                    html = _environment.Execute<string>(script);
                }
                catch (Exception ex)
                {
                    _environment.AddConsoleCall("error", string.Format("[Server Side Error] - ({0}) --- {1}", ComponentName, ex.InnerException.Message));
                }
            }

            return string.Format("<{0} {1}>{2}</{0}>", ContainerTag, attributes, html);
        }

        public string RenderJavascript(string clientGlobal)
        {
            return string.Format(
                "window.{0}.add('{1}', '{2}', {3});",
                clientGlobal,
                ComponentName, 
                ContainerId, 
                new HtmlString(_stringifiedProps)
            );
        }

        private static string GenerateId()
        {
            return "js-loader-" + Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", string.Empty)
                .Replace("+", string.Empty)
                .TrimEnd('=');
        }
    }
}