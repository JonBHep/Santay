   M S B A M L   `   `   `  ???? 
  Santay    Santay.GymWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -     ? Title$  
Gym visits??$	??850??$
??1400?? ? 
ResizeMode$ NoResize=? ? ShowInTaskbar$ False?? ? WindowStartupLocation$ CenterScreen=?.???  ? ColumnDefinitions ?? $	??260 ??? .+?]? .+??? -    ??Name$ DeleteButton??$??Bottom=?$??Right=?$
??12 3q?$??80??$??0 0 4 4q?.??Delete?? -    ??Name$ GymVisitsListBox??$??4q?
???????? $??1?? ? RowDefinitions ?? $	??156 ??? .+??  ?? $	??160 ??? $	??160 ???  ?? $??28 ??? .+??? -    ??Name$ YesterWeekTextBlock??$??Center=?$
??Bold"?$??6 0 0 0q?Week to yesterday?? -   $ TodayWeekTextBlock??$??0??$??1??$??Center=?$
??Bold"?$??6 0 0 0q?Week to today?? -   $ YesterWeekListBox??$??1??$??0??$??4q?
????????? -   $ TodayWeekListBox??$??1??$??1??$??4q?
???????? $??2??$??0??$??2?? ?? $??32 ??? ?? .+?]? $??False??.+??? -   $ 	AddButton??$??Center=?$
??12 3q?$??80??.??Add?? -	   $ CloseButton??$??Right=?$??0 0 6 0q?$??Center=?$
??12 3q?$
??Bold"?$??80??.??Close?? -
   	 ??Name$	 AddStackPanel??$??1??$??2??.+??? 
 ?? Orientation$
 
Horizontal=?$??0 12 0 0q?.+?	? -    	?Name$ GymRadio??$??Center=?$??0 0 10 0q?$	??100??.??Gym training	? -   $ 	AquaRadio??$??Center=?$??0 0 10 0q?$	??100??.??Aqua aerobics	? -   $ 
OtherRadio??$??Center=?$??0 0 10 0q?$	??100??.??Other activity(  "System.Windows.Controls.DatePicker  -     Name$ GymDatePicker??   IsTodayHighlighted$
 True??$??Center=?$	??200??$??0 0 10 0q??? -   $??Center=?$
??12 3q?$??80??$??Medium"?$??0 0 10 0q?.??OK?? -   $??Center=?$
??12 3q?$??80??.??Cancel?? $
 
Horizontal=?$??
220 12 0 0q?.+??? -   $??Center=?$
??12 3q?$??80??$??Medium"?$??0 0 10 0q?.??Today?? -   $??Center=?$
??12 3q?$??80??$??Medium"?$??0 0 10 0q?.??	Yesterday?? -   $??Center=?$
??12 3q?$??80??$??Medium"?$??0 0 10 0q? ? ToolTip$ The day before yesterday??.??
Avant-hier?? -   $??Center=?$
??12 3q?$??80??$??Medium"?$??0 0 10 0q?$ The day before yesterday??$??◄◄???? -   $??Center=?$
??12 3q?$??80??$??Medium"?$??0 0 10 0q?$ The day before yesterday??$??►►??? $??1?? ?? $??28 ??? $??68 ??? $??28 ??? ?? $??28 ??? ?? $??28 ??? ?? $??8 ?.+??? $??0??$??Sessions??$
??Bold"?$??Center=?$??8 0 0 0q??? -    ??Name$ SessScroller??$??1??$??Visible=?$??Hidden=?$??0 0 4 0q?.???? -    ??Name$ GraphCanvasSess??
????????? -   $	 	WeekPanel??$
 
Horizontal=?$??2???? -   $ WeekScroller??$??3??$??Visible=?$??Hidden=?$??0 0 4 0q?.???? -   $ GraphCanvasWeek??
????????? -   $	 	MnthPanel??$
 
Horizontal=?$??4???? -   $ MnthScroller??$??5??$??Visible=?$??Hidden=?$??0 0 4 0q?.???? -   $ GraphCanvasMnth??
????????? -   $	 	TotlPanel??$
 
Horizontal=?$??6???? -   $ TotlScroller??$??7??$??Visible=?$??Hidden=?$??0 0 4 0q?.???? -    $ GraphCanvasTotl??
???????