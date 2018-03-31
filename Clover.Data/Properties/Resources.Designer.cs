









namespace Clover.Data.Properties {
    using System;
    
    
    
    
    
    
    
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        
        
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Maticsoft.DBUtility.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        
        
        
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        
        
        
        internal static string SmartDateT {
            get {
                return ResourceManager.GetString("SmartDateT", resourceCulture);
            }
        }
        
        
        
        
        internal static string SmartDateToday {
            get {
                return ResourceManager.GetString("SmartDateToday", resourceCulture);
            }
        }
        
        
        
        
        internal static string SmartDateTom {
            get {
                return ResourceManager.GetString("SmartDateTom", resourceCulture);
            }
        }
        
        
        
        
        internal static string SmartDateTomorrow {
            get {
                return ResourceManager.GetString("SmartDateTomorrow", resourceCulture);
            }
        }
        
        
        
        
        internal static string SmartDateY {
            get {
                return ResourceManager.GetString("SmartDateY", resourceCulture);
            }
        }
        
        
        
        
        internal static string SmartDateYesterday {
            get {
                return ResourceManager.GetString("SmartDateYesterday", resourceCulture);
            }
        }
        
        
        
        
        internal static string StringToDateException {
            get {
                return ResourceManager.GetString("StringToDateException", resourceCulture);
            }
        }
        
        
        
        
        internal static string ValueNotSmartDateException {
            get {
                return ResourceManager.GetString("ValueNotSmartDateException", resourceCulture);
            }
        }
    }
}
