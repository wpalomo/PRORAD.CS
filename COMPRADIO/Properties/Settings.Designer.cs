﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace COMPRADIO.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DSN=SGANET;Description=SGA (.NET);UID=sa;PWD=angeles-sofia;APP=Microsoft® Visual " +
            "Studio® 2015;WSID=ABBE-ASUS;DATABASE=SGANET;QueryLog_On=Yes;StatsLog_On=Yes;")]
        public string ConnectString {
            get {
                return ((string)(this["ConnectString"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\PROYECTOS\\SGA.NET\\EXE\\RF\\RADIO.XML")]
        public string RfXmlPath {
            get {
                return ((string)(this["RfXmlPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\PROYECTOS\\SGA.NET\\EXE\\SGA\\SGA.XML")]
        public string SgaXmlFile {
            get {
                return ((string)(this["SgaXmlFile"]));
            }
        }
    }
}