   M S B A M L   `   `   `  ����  WebTerminalClient    WebTerminalClient.App   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        �� 	Resources  5          MyGlassGradientStopsResource � Color � Offset  MyGlassBrushResource �� 
StartPoint �� EndPoint �� Opacity  GrayBlueGradientBrush �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name �� RadiusX �� RadiusY J� Opacity J� RenderTransformOrigin J� BitmapEffect ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName �� Radius :� RoutedEvent ��Name �� 
TargetName �� TargetProperty	 W� By g� Duration �� BeginStoryboardName W� From	 W� To��%0  &	        &	 K    7   &	 �    '�� c    7  7  � 5   
   � 5      $ 
WhiteSmoke��6   $	 0.2Q�6.   � 5	      $ Transparent��6   $	 0.4Q�6/   � 5
      $ 
WhiteSmoke��6   $	 0.5Q�6.   � 5      $ Transparent��6   $
 0.75Q�6/   � 5      $ 
WhiteSmoke��6   $	 0.9Q�6.   � 5      $ Transparent��6   $ 1Q�6/   5      �� 5   
   $	 0,0&�5      $	 1,1&�6   $
 0.75Q�6,   8��  5      �� 5   
   $	 0,0&�5      $	 1,1&�6   .��5      � $ DarkGray��6   $ 0Q�6,   � 5      $ #CCCCFF��6   $	 0.5Q�6+   � 5      $ DarkGray��6   $ 1Q�6,   5      �� 5   
   " ��6   .�5      ��  � ��8   5      �� 5       � ��$	 300��6&   �� 5        � ��$ 64��6'   �� 5!       � ��$	 Top=�62   �� 5"       � ��$ 36(�6)   �� 5#       � ��$
 Bold"�6+   �� 5$       � �� 5%      �� 5&      "	 ��6&   .R�5'      � #��z��6   #��z��6?   }�. .+�5+      �� $
 outerRectangle��6(   $��Stretch=�6@   $��Stretch=�5,   #   #��z��6?   $ 20��5-   #   $ 20��60   $��5��6=   
������ 6Q   �� 50      $
 innerRectangle��6(   $��Stretch=�6@   $��Stretch=�51   #   
������ $��20��6T   #��z��52   #   $ 20��6G   $ 20��6T   �� 55      $
 	glassCube��6(   $��Stretch=�6;   $��Stretch=�56   #   $��2��57   #   $ 10��67   $ 10��6D   $ 1Q�6Q   8�� $ 0.5,0.5&�59   #   ��5:   "   �� 5;   &   $ 0.5,0&�6:   $ 0.5,1&�6M   ��5=   .   � $	 0.0Q�6;   $ 	LightBlue��6H   � 5>   .   $	 1.0Q�6;   $
 Gray��6H   5@   '   z�5F   "   V� 5G   &   .~�5H   *   �� �� 5I   *   5J   '    5O   "   �� 5P   &   5R      ]� 5U      $!��myContentPresenterDockPanel��6(   .+�5V   "   �� $ myContentPresenter��63   #��z��5W   %   ���	#FF364767$ Center=�5Y   %   $��Center=�6F   5Z      5[       5]      P� 5_       �@J�IsMouseOver . . �5e   "   �� $ outerRectangle��6F   # � ��5f   !    � ���� 5j   "   $ 	glassCube��6P    �@J�Opacity$ 1Q�6F   6P   �� 5n   "   $ myContentPresenter��5o   !    �@J�BitmapEffect 5p   &   �� 5q   *   $ 1Q�6;   5s   #   5t      P� 5v       �@J�	IsFocused . . �5w   "   �� $ 	glassCube��6V    �@J�Opacity$ 1Q�6F   6V   �� 5x   "   $ outerRectangle��6E   # � ��5y   %    � ���� 5z   "   $ 	glassCube��6P    �@J�Opacity$ 1Q�6F   6P   5{      :� 5~      $ Mouse.MouseEnter��6+   [�5   "   �� 5�   &   $ mouseEnterBeginStoryboard��66   .��5�   *   �� .��5�   .   W� $ 	glassCube��6>   $V P(Rectangle.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�5�   3   $
 -0.1X�5�   3   $ 0:0:0.5D�6=   W� 5�   .   $ 	glassCube��5�   1   $V P(Rectangle.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�5�   3   $
 -0.1X�5�   3   $ 0:0:0.5D�6=   5�   +   5�   '   5�      :� 5�      $ Mouse.MouseLeave��6+   [�5�   "   �� 5�   &   $ mouseEnterBeginStoryboard��65   5�      :� 5�      $ Button.Click��6+   [�5�   "   �� 5�   &   .��5�   *   �� .��5�   .   W� $ 	glassCube��6>   $ Opacity�5�   3   $ 0X�6W   $ 1X�6`   $ 0:0:0.5D�6g   5�   +   5�   '   5�      5�      5�      5�      5�      5�      