#pragma checksum "C:\Users\atoka\Desktop\dovizapp\dovizapp.webui\Views\Shared\Components\Navbar\Default.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "29d23003544db8b13a8f3d2a95a29539c14c4dd2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_Components_Navbar_Default), @"mvc.1.0.view", @"/Views/Shared/Components/Navbar/Default.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 3 "C:\Users\atoka\Desktop\dovizapp\dovizapp.webui\Views\_ViewImports.cshtml"
using dovizapp.webui.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"29d23003544db8b13a8f3d2a95a29539c14c4dd2", @"/Views/Shared/Components/Navbar/Default.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"afc70ddc4ea320b252ac447f2a7622a24992424a", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Shared_Components_Navbar_Default : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<bool>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"
<nav class=""navbar navbar-expand-md navbar-dark bg-primary"">
    <div class=""container"">
        <a href=""/"" class=""navbar-brand"" style=""font-weight: 600;""> <i class=""far fa-shopping-cart""></i> Döviz Uygulaması</a>

        <button type=""button"" class=""navbar-toggler"" data-bs-toggle=""collapse"" data-bs-target=""#navbarCollapse"" >
            <span class=""navbar-toggler-icon""></span>
        </button>

        <div class=""collapse navbar-collapse"" id=""navbarCollapse"">
            <ul class=""navbar-nav me-auto"">
                <li class=""nav-item ps-1 ps-sm-0"">
                    <a href=""/"" class=""nav-link active"">Tüm Dövizler</a>
                </li>              
            </ul>

");
#nullable restore
#line 18 "C:\Users\atoka\Desktop\dovizapp\dovizapp.webui\Views\Shared\Components\Navbar\Default.cshtml"
             if (Model == true)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <ul class=""navbar-nav ms-auto"">
                    <li class=""nav-item"">
                        <div class=""dropdown userDropdown"">
                            <button type=""button"" class=""btn btn-primary dropdown-toggle ps-1 ps-sm-2"" data-bs-toggle=""dropdown"" aria-expanded=""true""><i class=""fas fa-user-alt""></i> Hesabım</button>
                            <ul class=""dropdown-menu"">
                                <li><a href=""/Account/Logout"" class=""dropdown-item bg-danger text-white""><i class=""far fa-sign-out-alt""></i> Çıkış Yap</a></li>
                            </ul>                            
                        </div>
                    </li>
                </ul>
");
#nullable restore
#line 30 "C:\Users\atoka\Desktop\dovizapp\dovizapp.webui\Views\Shared\Components\Navbar\Default.cshtml"
            } else {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <ul class=""navbar-nav ms-auto"">
                    <li class=""nav-item ps-1 ps-sm-0"">
                        <a href=""/account/login"" class=""nav-link active""><i class=""far fa-sign-in-alt""></i> Giriş Yap</a>
                    </li>
                    <li class=""nav-item ps-1 ps-sm-0"">
                        <a href=""/account/register"" class=""nav-link active""><i class=""far fa-user-plus""></i> Kayıt Ol</a>
                    </li>
                </ul>
");
#nullable restore
#line 39 "C:\Users\atoka\Desktop\dovizapp\dovizapp.webui\Views\Shared\Components\Navbar\Default.cshtml"
            }            

#line default
#line hidden
#nullable disable
            WriteLiteral("        </div>\r\n    </div>\r\n</nav>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<bool> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
