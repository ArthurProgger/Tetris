   M S B A M L   `   `   `  ����A  =Tetris, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:TetrisTetris  5   	       Tetris.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Tetris     5   	   -   5          � Title$  Тетрис��5   	   $	��663��6   $	��682��6%    � WindowStartupLocation$ CenterScreen=�6y    � 
ResizeMode$ NoResize=�6�   ��5	      �� 5
   
    �� EndPoint$ 0.5,1&�6    � MappingMode$ RelativeToBoundingBox=�6/    �� 
StartPoint$ 0.5,0&�6S   .��5      �  � Color$ 	#FFFF9DFB��6   � 5      $ 	#FFA3FF77��6    � Offset$
 0.78Q�6-   5      .��5      � .+�5   
   �� -   5         ��Name$ 	startGame��5      	 J� Opacity$	 1Q�6$   
�������
����  �
 K� ZIndex$
 1��6m   .��6~   Начать игру6�   � 5   
   -   5         �Name$ 
gameGround��5      $	��632��6#   $��Bottom=�60   $
��Left=�6K   $	��392��6f   �� 5   
   -   5         ��Name$ comment��5      $	��150��6!   $��Arial)�6.   $
��Bold"�6A   $��15(�6S   $��213,303,46,181q�6a   $
 2��6y   ���	#FF00A49D$	 0Q�6�   $y�Hidden=�6�   .��6�   Комментарий6�   �� 5   
   -   5        $ 
nextFigLab��5      $��Right=�6$   $	��279��6@   $��45��6L   $	��Top=�6X   $��25(�6p   $��Arial)�6~   $
��Bold"�6�   .��6�   !Следующая фигура6�   � 5   
   -   5        $ nextFigureGround��5      $��392,45,0,305q�6)   $	��284��6?   $
��Left=�6L   $	��284��6g   �� 5   
   -   5        $ score��5      $��	0,0,10,10q�6   $��
Счет: ��62   $��Right=�6C   $	��282��6_   $��55��6k   $��Bottom=�6w   $��25(�6�   $��Arial)�6�   $
��Bold"�6�   6�   5      5      