   M S B A M L   `   `   `  ????
  Santay$    Santay.ListBloodPressureWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   -   5          ? Title$  Blood pressure readings??5   	   $	??600??6)   $
??1000??66    ? 
ResizeMode$ 	CanResize=?6C    ? WindowStartupLocation$ CenterScreen=?6Z    ? ShowInTaskbar$ False??6    ? 	Resources 5       ?? 
TargetType ?? Property ?? Value   TemplateDate  TemplateSystolic ?? TextAlignment  TemplateDiastolic  TemplatePulse??%f  '??       '?? M     &	  ?     &	 !    &	 z    &	 ?    ?? 5
   
   " ??6   .?5      ??  ? ??$ Stretch=?6;   5      ?? 5   
   " ??6   .?5      ??  ? ??$
 Left=?6;   ?? 5       ? ??$ 4q?6(   ?? 5       ? ??$ Medium"?6+   5      ?? 5   
   .R?5      ?? ??6   ?? *Date+5      ?? 5   
   .R?5      ?? $ Right=?62   ??6   ?? *
Systolic+62   5      ?? 5   
   .R?5      ?? $ Right=?63   ??6   ?? *	Diastolic+63   5      ?? 5   
   .R?5      ?? $ Right=?6/   ??6   ?? *Pulse+6/   5      5       .??5!      ? 	 ? RowDefinitions	 5"   
   ?? 5#      $
??Auto ?6   ?? 5$      .+?5&   
   ]? $??False??6   .+?5'      ?? -   5        
 ??Name$
 	ButtonAdd??5'      $	??8 3q?6(   $	??8 3q?65   $??60??6C   .??6f   Add6k   ?? 5(      -   5        $
 
ButtonEdit??5(      $	??8 3q?6)   $	??8 3q?66   $??60??6D   ${?False??6O   .??6z   Edit6?   ?? 5)      -   5        $
 ButtonDelete??5)      $	??8 3q?6+   $	??8 3q?68   $??60??6F   ${?False??6Q   .??6~   Delete6?   ?? 5*      -   5        $	??8 3q?5*      $	??8 3q?6"   $??60??60   $??Right=?6;   $
??Bold"?6R   .??6}   Close6?   5+      ?? 5,   
   -   5         ??Name$ TensionListBox??5,      $??1??6*   6e   57      58      