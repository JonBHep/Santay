   M S B A M L   `   `   `  ????
  Santay    Santay.ListWeightWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   -   5          ? Title$  Weight readings??5   	   $	??600??6!   $
??1000??6.    ? 
ResizeMode$ 	CanResize=?6;    ? WindowStartupLocation$ CenterScreen=?6R    ? ShowInTaskbar$ False??6w    ? 	Resources 5       ?? 
TargetType ?? Property ?? Value   TemplateDate  TemplateWeight  TemplatePounds ?? TextAlignment  TemplateKilograms  TemplateBmi??%?  '??       '?? M     &	  ?     &	 !    &	 b    &	 ?    &	     '?? g    ?? 5
   
   " ??6   .?5      ??  ? ??$ Stretch=?6;   5      ?? 5   
   " ??6   .?5      ??  ? ??$
 Left=?6;   ?? 5       ? ??$ 4q?6(   ?? 5       ? ??$ Medium"?6+   5      ?? 5   
   .R?5      ?? ??6   ?? *Date+5      ?? 5   
   .R?5      ?? ??6   ?? *Weight+5      ?? 5   
   .R?5      ?? $ Right=?60   ??6   ?? *Pounds+60   5      ?? 5   
   .R?5      ?? $ Right=?63   ??6   ?? *	Kilograms+63   5      ?? 5   
   .R?5       ?? $ Right=?6-   ??6   ?? *Bmi+6-   5!      ?? 5#   
   " ??6   .?5$      ??  ? ??$ 96??6)   66   ?? 5%       ? ??$ 26??6'   64   ?? 5&       ? ??
 ?????6;   ?? 5'       ? ??
 ? d ?6?   ?? 5(       ? ??$ Center=?62   6C   ?? 5)       ? ??$ 2 0 2 0q?6'   69   5*      5+      .??5-      ? 	 ? RowDefinitions	 5.   
   ?? 5/      $??30 ?6   ?? 50      $??30 ?6   ?? 51      .+?54   
   ]? $??0??6   $??False??6"   .+?55      ?? -   5        $??Right=?55      $
??Bold"?6,   .??6W   Close6^   ?? 56      -   5        $??Center=?56      $??6 0  0 0q?60   $??Medium"?6B   .??6r   Height6z   ?? 57      -   5        
 ??Name$
 HeightTextBlock??57      $??Center=?61   $??12 0 0 0q?6L   $
??Bold"?6^   Height6y   58      ]? 5:   
   $??1??6   $??False??6!   .+?5;      ?? -   5        $??6 0 0 0q?5;      .??6=   Add6B   ?? 5<      -   5         ??Name$ 
ButtonEdit??5<      ${?False??6)   .??6T   Edit6Z   ?? 5=      -   5        $ ButtonDelete??5=      ${?False??6+   .??6X   Delete6`   5?      ?? 5A   
   -   5         ??Name$ ListviewData??5A      $??2??6)    ?? View 5B      ?? 5C      .P?5D      ??  ?? Width$
 Auto??6%   $
??Date??62    ?? CellTemplate# [  6@   ?? 5E      $
 Auto??6%   $??Weight??62   # [ 6B   ?? 5F      $
 Auto??6%   $??Pounds??62   # [ 6B   ?? 5G      $
 Auto??6%   $??	Kilograms??62   # [ 6E   ?? 5H      $
 Auto??6%   $	??BMI??62   # [ 6?   5I      5K      5L      5N      