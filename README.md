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
  <DataTemplate DataType="{x:Type local:DataItem}" x:Key="dataItemTemplate">
    <Grid Opacity="0.8">
      <Rectangle Fill="{Binding Path=Color}"/>
    </Grid>
  </DataTemplate>
</YourView.Resources>

<Grid Height="260" Width="420">
  ...
  <vp:AutoScrollableView
    Items="{x:Static local:DataItem.DataItems}"
    ItemTemplate="{StaticResource dataItemTemplate}"
    x:Name="scrollableView"
    ScrollBehaviour="Backward"
    ScrollDurationSec="0.4"
    ScrollIntervalSec="5"
    Orientation="Horizontal"/>
  ...
</Grid>
</YourView>
```

You can control this view with follow property:<br>
  |Property|Property Type|Description|
  |:---|:---|:---|
  |**Items**|DependencyProperty|specify items to scroll|
  |**ItemTemplate**|DependencyProperty|specify a DataTemplate for item|
  |**Orientation**|Property|define the scroll orientation(horizontal or vertical)|
  |**ScrollBehaviour**|Property|define whether scroll backward or scroll forward|
  |**ScrollDurationSec**|Property|how long(in seconds) the scroll animation take|
  |**ScrollIntervalSec**|Property|how long(in seconds) the current item keep|
  
