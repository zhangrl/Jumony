﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 用于渲染 view 标签的元素渲染代理
  /// </summary>
  public class ViewElementAdapter : HtmlElementAdapter
  {

    private ViewContext _context;

    /// <summary>
    /// 创建 ViewElementAdapter 对象
    /// </summary>
    /// <param name="context"></param>
    public ViewElementAdapter( ViewContext context )
    {
      _context = context;
    }


    /// <summary>
    /// 渲染 view 标签
    /// </summary>
    /// <param name="element">view 标签元素</param>
    /// <param name="writer">HTML 编写器</param>
    public override void Render( IHtmlElement element, TextWriter writer )
    {

      var key = element.Attribute( "key" ).Value() ?? element.Attribute( "name" ).Value();

      object dataObject;
      if ( key != null )
        _context.ViewData.TryGetValue( key, out dataObject );
      else
        dataObject = _context.ViewData.Model;


      if ( dataObject == null )
        return;


      string bindValue = null;

      var path = element.Attribute( "path" ).Value();
      var format = element.Attribute( "format" ).Value();


      if ( path == null )
        bindValue = string.Format( format ?? "{0}", dataObject );
      else
        bindValue = DataBinder.Eval( dataObject, path, format ?? "{0}" );


      var attributeName = element.Attribute( "attribute" ).Value() ?? element.Attribute( "attr" ).Value();
      if ( attributeName != null )
      {
        element.NextElement().SetAttribute( attributeName, bindValue );
        return;
      }

      var variableName = element.Attribute( "var" ).Value() ?? element.Attribute( "variable" ).Value();
      if ( variableName != null )
      {
        var hostName = element.Attribute( "host" ).Value();
        if ( hostName == null )
          writer.WriteLine( "<script type=\"text/javascript\">window['{0}'] = '{1}';</script>", variableName, bindValue );

        else
          writer.WriteLine( "<script type=\"text/javascript\">window['{0}']['{1}'] = '{2}';</script>", hostName, variableName, bindValue );

        return;
      }

      writer.Write( bindValue );

    }


    /// <summary>
    /// 用于匹配 view 标签的 CSS 选择器
    /// </summary>
    protected override string CssSelector
    {
      get { return "view"; }
    }

  }
}