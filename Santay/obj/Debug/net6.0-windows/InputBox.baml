   M S B A M L   `   `   `  ????
  Santay    Santay.InputBox   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   -   5          ? Title$  	Input box??5   	   $	??100??6   $	??500??6(    ? ShowInTaskbar$ False??64    ? Topmost$
 True??6J    ? WindowStartupLocation$ CenterOwner=?6Y    ? SizeToContent$ Width=?6}    ? 
ResizeMode$ NoResize=?6?   .??5      ? 
??????? ? ColumnDefinitions 5	   
   ?? 5
      $
??Auto ?6   ?? 5      $
??Auto ?6    ? RowDefinitions 5   
   ?? 5      ?? 5      .+?5   
   ?? -   5         ??Name$ TextblockPrompt??5      $??Center=?6-   
???p?$??6,2,6,2q?6b   
[prompt]6}   ?? 5   
   -   5        	 ??Name$	 TextboxResponse??5      $??0??6+   $??1??68   $??Center=?6H   $	??300??6c   $??6,2,6,2q?6o   $	??4 2q?6?   $??Center=?6?   $??Medium"?6?   ]? 5   
   $??1??6   $??1??6!   $??False??61   .+?5      ?? -   5        $??Right=?5      $
??Bold"?6,   $??Center=?6>   $??8,3,8,3q?6Y   $	??4 2q?6j   $??80??6?   .??6?   OK6?   ?? 5      -   5        $??Right=?5      $??Center=?6,   $??8,3,8,3q?6G   $	??4 2q?6X   $??80??6?   .??6?   Cancel6?   5      5      5      