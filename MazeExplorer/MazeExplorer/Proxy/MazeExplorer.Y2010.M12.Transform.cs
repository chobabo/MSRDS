//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:2.0.50727.4927
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
[assembly: global::System.Reflection.AssemblyProductAttribute("MazeExplorer")]
[assembly: global::System.Reflection.AssemblyTitleAttribute("MazeExplorer")]
[assembly: global::Microsoft.Dss.Core.Attributes.ServiceDeclarationAttribute(global::Microsoft.Dss.Core.Attributes.DssServiceDeclaration.Transform, SourceAssemblyKey="MazeExplorer.Y2010.M12, Version=1.0.0.0, Culture=neutral, PublicKeyToken=cc364a89" +
    "174f4986")]
[assembly: global::System.Security.SecurityTransparentAttribute()]
[assembly: global::System.Security.AllowPartiallyTrustedCallersAttribute()]

namespace Dss.Transforms.TransformMazeExplorer {
    
    
    public class Transforms : global::Microsoft.Dss.Core.Transforms.TransformBase {
        
        static Transforms() {
            Register();
        }
        
        public static void Register() {
            global::Microsoft.Dss.Core.Transforms.TransformBase.AddProxyTransform(typeof(global::MazeExplorer.Proxy.MazeExplorerState), new global::Microsoft.Dss.Core.Attributes.Transform(MazeExplorer_Proxy_MazeExplorerState_TO_MazeExplorer_MazeExplorerState));
            global::Microsoft.Dss.Core.Transforms.TransformBase.AddSourceTransform(typeof(global::MazeExplorer.MazeExplorerState), new global::Microsoft.Dss.Core.Attributes.Transform(MazeExplorer_MazeExplorerState_TO_MazeExplorer_Proxy_MazeExplorerState));
        }
        
        private static global::MazeExplorer.Proxy.MazeExplorerState _cachedInstance0 = new global::MazeExplorer.Proxy.MazeExplorerState();
        
        private static global::MazeExplorer.MazeExplorerState _cachedInstance = new global::MazeExplorer.MazeExplorerState();
        
        public static object MazeExplorer_Proxy_MazeExplorerState_TO_MazeExplorer_MazeExplorerState(object transformFrom) {
            return _cachedInstance;
        }
        
        public static object MazeExplorer_MazeExplorerState_TO_MazeExplorer_Proxy_MazeExplorerState(object transformFrom) {
            return _cachedInstance0;
        }
    }
}
