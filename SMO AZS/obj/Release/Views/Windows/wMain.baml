   M S B A M L   `   `   `  ���� B  >SMO AZS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:SMO_AZS.Views.WindowsSMO_AZS.Views.Windows  !    SMO_AZS.Views.Windows.wMain   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5local#clr-namespace:SMO_AZS.Views.Windows     -   $	��600��$	��800��$	��450��$	��500��  � WindowStartupLocation$  CenterScreen=� � Title ��  �� ElementName$ FrMain�� �� Path$ Content.Title� � 	Resources  �� 
TargetType   MaterialDesignFlatButton �� BasedOn �� Property �� Value��%c   '��       7   �� " ��8   .���  � ��
 ���� ��  � ��$ 0q���  � ��$ 50��.��� 	 � RowDefinitions	 �� $
��auto ��� $
��auto ��� .+�o� .@�m� $��Ввод данных��.@�m� -   
 m�Name$
 MIManualInput��$��Вручную��m� -   $
 MIFileInput��$��Из файла��m� -   $
 MIOutputResult��$.��(Вывод выходных данных��m� $��Справка��.@�m� -   $
 MIAbout��$��О программе��m� -   $
 MIHelp��$��Руководство���� $��1�� �� Orientation$ 
Horizontal=�$��5q�.+��� -    ��Name$ BtnBack����F� 
���   �$��1�� �� Stretch$ Uniform=� �� StrokeStartLineCap$ Round=� �� StrokeEndLineCap$ Round=� �� StrokeLineJoin$ Round=�4���� z     �     z � $�  z �     z � z  $� 	�� -   $ 
BtnForward��$��RightToLeft=�$	��5 0q���F� 
���   �$��1��$ Uniform=�$ Round=�$ Round=�$ Round=�4���� z     �     z � $�  z �     z � z  $� 	 � -	     �Name$ FrMain��$��2��  � Source$ ../Pages/pAbout.xaml?�  � NavigationUIVisibility$ Hidden=�