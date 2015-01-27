"../../packages/OpenCover.4.5.3522/OpenCover.Console.exe" -target:"../../packages/NUnit.Runners.2.6.4/tools/nunit-console.exe" -targetargs:"/nologo DXGameTest.dll /noshadow" -filter:"+[DXGame]DXGame*" -excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" -register:user -output:"CodeCoverageResult.xml"

"../../packages/ReportGenerator.2.0.4.0/ReportGenerator.exe" "-reports:CodeCoverageResult.xml" "-targetdir:CodeCoverageReport"

start file://%~dps0CodeCoverageReport/index.htm