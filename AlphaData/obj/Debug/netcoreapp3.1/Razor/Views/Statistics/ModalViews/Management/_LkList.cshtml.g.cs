#pragma checksum "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "b926ed42965849801d8270b12ea52f67d8d061630d8dd965bb350f99e57341cb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Statistics_ModalViews_Management__LkList), @"mvc.1.0.view", @"/Views/Statistics/ModalViews/Management/_LkList.cshtml")]
namespace AspNetCore
{
    #line default
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Mvc.Rendering;
    using global::Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
 using AlphaData.Models.Statistics.Management

#line default
#line hidden
#nullable disable
    ;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"b926ed42965849801d8270b12ea52f67d8d061630d8dd965bb350f99e57341cb", @"/Views/Statistics/ModalViews/Management/_LkList.cshtml")]
    #nullable restore
    public class Views_Statistics_ModalViews_Management__LkList : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<LkModel>>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 4 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
 foreach (LkModel lkModel in Model)
{

#line default
#line hidden
#nullable disable

            WriteLiteral("<div class=\"lk\">\r\n    <div class=\"lk__info\">\r\n        <span class=\"lk__name\">");
            Write(
#nullable restore
#line 8 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
                                lkModel.name

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("</span> <span>");
            Write(
#nullable restore
#line 8 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
                                                           lkModel.login

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("</span>\r\n    </div>\r\n    <div class=\"lk__btn\">\r\n        <input type=\"button\" class=\"btn remove\" data-idlk=\"");
            Write(
#nullable restore
#line 11 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
                                                            lkModel.id

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\" value=\"Удалить\" />\r\n        <input type=\"button\" class=\"btn edit\" data-idlk=\"");
            Write(
#nullable restore
#line 12 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
                                                          lkModel.id

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\" value=\"Задачи\" />\r\n        <input type=\"button\" class=\"btn share\" data-idlk=\"");
            Write(
#nullable restore
#line 13 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
                                                           lkModel.id

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\" value=\"Открыть\" />\r\n    </div>\r\n</div>\r\n");
#nullable restore
#line 16 "A:\Проекты работа\AlphaData\AlphaData\alphaData\Views\Statistics\ModalViews\Management\_LkList.cshtml"
}

#line default
#line hidden
#nullable disable

            WriteLiteral("\r\n");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<LkModel>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
