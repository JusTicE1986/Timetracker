   M S B A M L   `   `   `  ����  Timetracker/    )Timetracker.Views.WochenUebersichtControl   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5
      ��    	CardStyle#��[  6   ._�5   
   �� .+�5      �� $��Wochenübersicht��6   $��18(�5      $��SemiBold"�5      $��0 0 0 10q�5      �� 5        �� Orientation$  
Horizontal=�6   $��Center=�5      $��0 0 0 10q�5      .+�5      �� $	��◀��6   $��30��5      ��5      �� *WocheZurueckCommand+5      �� 5      $��16(�6   $��SemiBold"�5      $
��10,0q�5      $��Center=�5      ��5      �� *KalenderwochenAnzeige+�� 5      $	��▶��6   $��30��5       ��5      �� *WocheVorCommand+5       5!      &   System.Windows.Controls.DataGrid  5$         AutoGenerateColumns$ False��5&         
IsReadOnly$
 True��5'      $��	0 10 0 10q�5(         CanUserAddRows$ False��5)         SelectionMode$ Single=�5*         SelectionUnit$ FullRow=�5+      ��5$      �� *WochenDaten+ �� SelectedItem 5%      �� *AusgewaehlterTag+ �� Mode$ TwoWay=�   RowStyle 5,      �� 5-      	 �� 
TargetType)  #System.Windows.Controls.DataGridRow"	  6   .�5.      �� 
 �� Property
 �@�Cursor �� Value$
 Hand��63   �� 5/      
 �@�ToolTip$' !Klicken zum Anzeigen / Bearbeiten��64   �� 50      
 � ��$ 0q�63   �� 51      
 � ��$ 4q�64   �� 52      
 �@J�SnapsToDevicePixels$
 True��6@    �� Triggers 53      P� 54       P� Property �@J�IsMouseOver P� Value$
 True��6=   . �55   "   �� 
 � �� �#dceeff6?   56      �� 57       �� Binding 6*   �� *IstFeiertag+ �� Value True6J   .a�58   "   �� 
 � �� �#ffe4e16?   59      �� 5:       6*   �� *IstWochenende+ True6L   .a�5;   "   �� 
 � �� �#e8ffe86?   5<      5>         Columns 5A      0  *System.Windows.Controls.DataGridTextColumn  5B      ,  &System.Windows.Controls.DataGridColumn   Header$ Datum��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 5D   )   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5C   )   �� *Datum+ �� StringFormat$ 
dd.MM.yyyy��5D   )     5E      $	 Tag��6)   $	 100 5G   )    5F   )   �� *	Wochentag+5G   )     5H      $ Pause��6)     TimeSpanStyle   ElementStyle# [ 5J   )   $	 100 5K   )    5I   )   �� *BerechnetePause+$ hh\:mm��5K   )   4  .System.Windows.Controls.DataGridTemplateColumn  5L      $ Erfasst��6-   $	 100 6>      CellTemplate 5M      �� 5N      .R�5O   "   �� $��16(�6,    �� TextAlignment$ Center=�5P   ,   $��Center=�5Q   ,   ��5R   ,   �� *
IstErfasst+  BoolToIconConverter �� 	Converter# [ 5S      5U        5V      $ 
Gearbeitet��6)   $	 100 5X   )    5W   )   �� *BerechneteGearbeiteteZeit+$ hh\:mm��5X   )     5Y      $ Besonderheit��6)   $ * 5[   )    5Z   )   �� *Besonderheit+5[   )   5]      �� 5`      $
��Bold"�5a      $��16(�5b      $��Right=�5c      $��0 0 0 10q�5d      ��5`      �� *WochenSumme+  ReadableTextConverter# [ 5d      5e      5f      5g      