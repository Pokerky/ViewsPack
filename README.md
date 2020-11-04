# ViewsPack
This project contains some interesting views for WPF .Net Core

## AutoScrollableView
This view can scroll items automatically

```
<YourView
...
xmlns:vp="clr-namespace:ViewsPack;assembly=ViewsPack"
...>

<YourView.Resources>
  <DataTemplate DataType="{x:Type local:DataItem}">
    <Grid Opacity="0.8">
      <Rectangle Fill="{Binding Path=Color}"/>
    </Grid>
  </DataTemplate>
</YourView.Resources>

<vp:AutoScrollableView
  Items="{x:Static local:DataItem.DataItems}"
  x:Name="scrollableView"
  ScrollBehaviour="Backward"
  ScrollDurationSec="0.4"
  ScrollIntervalSec="5"
  Orientation="Horizontal" 
  Width="400" Height="240"/>

</YourView>
```

The **Width** and **Height** have to be assigned to a specified value, then you can control this view with some property:<br>
  **Items**(DependencyProperty): specify items to scroll<br>
  **Orientation**(property): define the scroll orientation<br>
  **ScrollBehaviour**(property): define whether scroll backward or scroll forward<br>
  **ScrollDurationSec**(property): how long(in seconds) the scroll animation take<br>
  **ScrollIntervalSec**(property): how long(in seconds) the current item keep
  
