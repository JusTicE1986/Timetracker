   M S B A M L   `   `   `  ����F  BTimetracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Timetracker.ViewsTimetracker.Views  5   	       Timetracker.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1viewsclr-namespace:Timetracker.Views     5   	     � Title$  Timetracker��5   	   $
��1024��5	   	   $	��768��6   $
��1024��5
   	   $	��600��6      AppBackgroundBrush#���   5   	   $��Segoe UI Variable)�5   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   "   Timetracker.Views.ExportCard  $��0��6   � 5   
   $��1��6   $��20 10q�6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      -   'Timetracker.Views.ZeitErfassungsControl  $��0��6*   $��0 0 20 0q�6:   )   #Timetracker.Views.MonatsInfoControl  5      $��1��6&   5       /   )Timetracker.Views.WochenUebersichtControl  5#   
   $��2��6(   $
��20 0q�65   �� 5&   
   -   5         ��Name$ ThemeToggleButton��5&      $��🌙 Dark Mode��5'      $	��120��5(      $��30��5)      $��Right=�5*      $	��Top=�5+      $��	0,10,40,0q�5,      5-      5.      5/      