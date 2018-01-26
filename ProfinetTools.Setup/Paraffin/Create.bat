mkdir ..\Fragments
Paraffin.exe -dir ..\..\out\Release ^
             -NoRootDirectory ^
             -alias $(var.SourcePath) ^
             -GroupName ProfinetToolsFragments ..\Fragments\Fragments.wxs ^
             -ext .json ^
             -ext .sdf ^
             -regExExclude "\.vshost\." ^
             -regExExclude "\.dll\.conf"
