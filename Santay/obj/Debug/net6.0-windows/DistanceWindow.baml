   M S B A M L   `   `   `  ????
  Santay    Santay.DistanceWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   -   5          ? Title$*  $Velo - Rolling daily distance cycled??5   	   $	??500??66   $
??1000??6C    ? WindowStartupLocation$ CenterScreen=?6P    ? 
ResizeMode$ NoResize=?6u   $??Consolas)?6?    ? ShowInTaskbar$ False??6?   .??5      ?  ? RowDefinitions 5	   
   ?? 5
      $
??Auto ?6   ?? 5      .+?5   
   ]? .+?5      ?? $??80??6   $
??12 4q?6    $??4q?6/   $??Right=?6:    ?? IsCancel$
 True??6Q   .??6a   OK6e   ?? 5      $??30(?6   $??
AR Essence)?6'   $??12 0 0 0q?6?   
???????Distance and Frequency6?   ?? 5      $??20(?6   $??Center=?6'   $??
AR Essence)?6B   $??12 0 0 0q?6Z   
???????53Upper chart: Km per day Lower chart: rides per week6?   ?? 5      -   5         ??Name$ YearBloc??5      $??14(?6+   $??Center=?69   $??12 0 0 0q?6T   
???  ??Rolling year6?   ?? 5      -   5        $ Bloc90??5      $??14(?6)   $??Center=?67   $??12 0 0 0q?6R   
????  ?Rolling 90 days6?   ?? 5      -   5        $ Bloc30??5      $??14(?6)   $??Center=?67   $??12 0 0 0q?6R   
????? ?Rolling 30 days6?   ?? 5      $??14(?6   $??Center=?6'   $??12 0 0 0q?6B   
???   ?%#Dotted lines represent latest value6?   5      ?? 5   
   -   5         ??Name$ ChartScrollViewer??5      $??1??62   $
??Auto=?6?   .??5      ?? -   5         ??Name$ ChartCanvas??5      
???????$??3q?6D   5      5      5      