using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ViewsPack
{

	public enum ScrollBehaviour
	{
		Forward,
		Backward
	}


	/// <summary>
	/// ScrollableView.xaml µÄ½»»¥Âß¼­
	/// </summary>
	public partial class AutoScrollableView : UserControl
	{
		#region Offset depedency property
		public double Offset
		{
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}

		public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
			"Offset", typeof(double), typeof(AutoScrollableView),
			new PropertyMetadata(0.0, OnOffsetPropertyChanged)
			);

		private static void OnOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is AutoScrollableView source)
			{
				source.OnOffsetPropertyChanged(e);
			}
		}

		protected void OnOffsetPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			// scroll vertical when orientation is vertical
			// while scroll horizontal when orientation is horizontal
			if (Orientation == Orientation.Vertical)
			{
				scrollViewer.ScrollToVerticalOffset(Offset);
			}
			else
			{
				scrollViewer.ScrollToHorizontalOffset(Offset);
			}
		}

		#endregion

		#region ItemTeplate dependency property
		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
			"ItemTemplate", typeof(DataTemplate), typeof(AutoScrollableView));

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		#endregion

		#region Items dependency property
		public object[] Items
		{
			get { return (object[])GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
			"Items", typeof(object[]), typeof(AutoScrollableView),
			new PropertyMetadata(null, OnItemsPropertyChanged)
			);

		private static void OnItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is AutoScrollableView source)
			{
				source.OnItemsPropertyChanged(e);
			}
		}

		protected void OnItemsPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is object[] items && items.Length > 0)
			{
				// exapand items
				List<object> itemsExpand = new List<object>();
				itemsExpand.Add(items.Last());
				itemsExpand.AddRange(items);
				itemsExpand.Add(items.First());

				itemsHost.ItemsSource = itemsExpand;
			}
			else
			{
				itemsHost.ItemsSource = null;
			}

			// reset status when items changed
			Reset();
		}

		#endregion

		private readonly DispatcherTimer _scrollTimer = new DispatcherTimer();
		private bool _isScrolling = false;
		// recording current showing index in itemsHost, 
		// whose source is expanded
		private int _currentIndex = 0;

		public AutoScrollableView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// auto scroll forward or auto scroll backward
		/// </summary>
		public ScrollBehaviour ScrollBehaviour { get; set; } = ScrollBehaviour.Backward;

		/// <summary>
		/// duration for scroll to next item or previous item
		/// </summary>
		public double ScrollDurationSec { get; set; } = 0.4;

		/// <summary>
		/// interval for auto scroll
		/// </summary>
		public double ScrollIntervalSec { get; set; } = 3;

		/// <summary>
		/// orientation for scroll
		/// </summary>
		public Orientation Orientation { get; set; } = Orientation.Vertical;

		/// <summary>
		/// step for change offset
		/// </summary>
		private double OffsetStep
		{
			get
			{
				if (Orientation == Orientation.Horizontal) { return ActualWidth; }
				else { return ActualHeight; }
			}
		}

		#region method for scroll item
		/// <summary>
		/// scroll to next item
		/// </summary>
		public void ScrollToNextItem()
		{
			if (_isScrolling) { return; }
			if (Items == null || Items.Length == 0) { return; }

			// Console.WriteLine("scroll to next");
			_isScrolling = true;

			DoubleAnimation animation = CreateScrollAnimation();
			animation.From = Offset;
			animation.To = Offset + OffsetStep;
			animation.Completed += ScrollNextAnimation_Completed;

			this.BeginAnimation(OffsetProperty, animation);
		}

		/// <summary>
		/// change status when scroll next animation completed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScrollNextAnimation_Completed(object sender, EventArgs e)
		{
			if (Items == null || Items.Length == 0) { return; }

			_currentIndex += 1;
			if (_currentIndex >= Items.Length + 1)
			{
				// reach the last one,
				// we need set Offset to show the first item

				// Console.WriteLine("reach the last one");
				// remove the effection of animation
				this.BeginAnimation(OffsetProperty, null);
				// reset status
				_currentIndex = 1;
				Offset = OffsetStep * _currentIndex;
			}
			//  else index is valid

			// change select index of overallBar
			// there is a expanded item, so need to minus one
			overallBar.SelectedIndex = _currentIndex - 1;

			_isScrolling = false;
		}

		/// <summary>
		/// scroll to previous item
		/// </summary>
		public void ScrollToPreviousItem()
		{
			if (_isScrolling) { return; }
			if (Items == null || Items.Length == 0) { return; }

			// Console.WriteLine("scroll to previous");
			_isScrolling = true;

			DoubleAnimation animation = CreateScrollAnimation();
			animation.From = Offset;
			animation.To = Offset - OffsetStep;
			animation.Completed += ScrollPreviousAnimation_Completed;

			this.BeginAnimation(OffsetProperty, animation);
		}

		/// <summary>
		/// change status when scroll previous animation completed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScrollPreviousAnimation_Completed(object sender, EventArgs e)
		{
			if (Items == null || Items.Length == 0) { return; }

			_currentIndex -= 1;
			if (_currentIndex <= 0)
			{
				// reach the first one, 
				// we need set Offset to show the last item

				// Console.WriteLine("reach the first one");
				// remove the effection of animation
				this.BeginAnimation(OffsetProperty, null);
				// reset status
				_currentIndex = Items.Length;
				Offset = OffsetStep * _currentIndex;
			}
			//  else index is valid

			// change select index of overallBar
			// there is a expanded item, so need to minus one
			overallBar.SelectedIndex = _currentIndex - 1;

			_isScrolling = false;
		}

		/// <summary>
		/// create animation for scroll
		/// </summary>
		/// <returns></returns>
		private DoubleAnimation CreateScrollAnimation()
		{
			DoubleAnimation offsetAnimation = new DoubleAnimation();
			offsetAnimation.Duration = TimeSpan.FromSeconds(ScrollDurationSec);
			// offsetAnimation.FillBehavior = FillBehavior.Stop;

			return offsetAnimation;
		}

		#endregion

		/// <summary>
		/// auto scroll forward or backward
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScrollTimer_Tick(object sender, EventArgs e)
		{
			if (ScrollBehaviour == ScrollBehaviour.Forward) { ScrollToPreviousItem(); }
			else { ScrollToNextItem(); }
		}

		#region control buttons
		private Thickness LeftHideMargin
		{
			get { return new Thickness(-leftCtrlBtn.Width, 0, 0, 0); }
		}

		private Thickness LeftShowMargin
		{
			get { return new Thickness(0, 0, 0, 0); }
		}

		private Thickness TopHideMargin
		{
			get { return new Thickness(0, -topCtrlBtn.Height, 0, 0); }
		}

		private Thickness TopShowMargin
		{
			get { return new Thickness(0, 0, 0, 0); }
		}

		private Thickness RightHideMargin
		{
			get { return new Thickness(ActualWidth, 0, 0, 0); }
		}

		private Thickness RightShowMargin
		{
			get { return new Thickness(ActualWidth - rightCtrlBtn.Width, 0, 0, 0); }
		}

		private Thickness BottomHideMargin
		{
			get { return new Thickness(0, ActualHeight, 0, 0); }
		}

		private Thickness BottomShowMargin
		{
			get { return new Thickness(0, ActualHeight - bottomCtrlBtn.Height, 0, 0); }
		}

		/// <summary>
		/// hide control buttons
		/// </summary>
		private void HideControlButtons()
		{
			if (leftCtrlBtn == null || topCtrlBtn == null || rightCtrlBtn == null || bottomCtrlBtn == null) { return; }

			// remove effection of animation
			leftCtrlBtn.BeginAnimation(MarginProperty, null);
			topCtrlBtn.BeginAnimation(MarginProperty, null);
			rightCtrlBtn.BeginAnimation(MarginProperty, null);
			bottomCtrlBtn.BeginAnimation(MarginProperty, null);
			// re-set margin of button
			leftCtrlBtn.Margin = LeftHideMargin;
			topCtrlBtn.Margin = TopHideMargin;
			rightCtrlBtn.Margin = RightHideMargin;
			bottomCtrlBtn.Margin = BottomHideMargin;
		}

		/// <summary>
		/// slide show control buttons
		/// </summary>
		private void SlideShowControlButtons()
		{
			if (leftCtrlBtn == null || topCtrlBtn == null || rightCtrlBtn == null || bottomCtrlBtn == null) { return; }

			if (Orientation == Orientation.Horizontal)
			{
				ThicknessAnimation leftBtnAnimation = CreateSlideAnimation();
				leftBtnAnimation.From = leftCtrlBtn.Margin;
				leftBtnAnimation.To = LeftShowMargin;
				Storyboard.SetTarget(leftBtnAnimation, leftCtrlBtn);
				Storyboard.SetTargetProperty(leftBtnAnimation, new PropertyPath(Button.MarginProperty));

				ThicknessAnimation rightBtnAnimation = CreateSlideAnimation();
				rightBtnAnimation.From = rightCtrlBtn.Margin;
				rightBtnAnimation.To = RightShowMargin;
				Storyboard.SetTarget(rightBtnAnimation, rightCtrlBtn);
				Storyboard.SetTargetProperty(rightBtnAnimation, new PropertyPath(Button.MarginProperty));

				Storyboard storyboard = new Storyboard();
				storyboard.Children.Add(leftBtnAnimation);
				storyboard.Children.Add(rightBtnAnimation);
				storyboard.Begin(this);
			}
			else
			{
				ThicknessAnimation topBtnAnimation = CreateSlideAnimation();
				topBtnAnimation.From = topCtrlBtn.Margin;
				topBtnAnimation.To = TopShowMargin;
				Storyboard.SetTarget(topBtnAnimation, topCtrlBtn);
				Storyboard.SetTargetProperty(topBtnAnimation, new PropertyPath(Button.MarginProperty));

				ThicknessAnimation bottomBtnAnimation = CreateSlideAnimation();
				bottomBtnAnimation.From = bottomCtrlBtn.Margin;
				bottomBtnAnimation.To = BottomShowMargin;
				Storyboard.SetTarget(bottomBtnAnimation, bottomCtrlBtn);
				Storyboard.SetTargetProperty(bottomBtnAnimation, new PropertyPath(Button.MarginProperty));

				Storyboard storyboard = new Storyboard();
				storyboard.Children.Add(topBtnAnimation);
				storyboard.Children.Add(bottomBtnAnimation);
				storyboard.Begin(this);
			}
		}

		/// <summary>
		/// slide hide control buttons
		/// </summary>
		private void SlideHideControlButtons()
		{
			if (leftCtrlBtn == null || topCtrlBtn == null || rightCtrlBtn == null || bottomCtrlBtn == null) { return; }

			if (Orientation == Orientation.Horizontal)
			{
				ThicknessAnimation leftBtnAnimation = CreateSlideAnimation();
				leftBtnAnimation.From = leftCtrlBtn.Margin;
				leftBtnAnimation.To = LeftHideMargin;
				Storyboard.SetTarget(leftBtnAnimation, leftCtrlBtn);
				Storyboard.SetTargetProperty(leftBtnAnimation, new PropertyPath(Button.MarginProperty));

				ThicknessAnimation rightBtnAnimation = CreateSlideAnimation();
				rightBtnAnimation.From = rightCtrlBtn.Margin;
				rightBtnAnimation.To = RightHideMargin;
				Storyboard.SetTarget(rightBtnAnimation, rightCtrlBtn);
				Storyboard.SetTargetProperty(rightBtnAnimation, new PropertyPath(Button.MarginProperty));

				Storyboard storyboard = new Storyboard();
				storyboard.Children.Add(leftBtnAnimation);
				storyboard.Children.Add(rightBtnAnimation);
				storyboard.Begin(this);
			}
			else
			{
				ThicknessAnimation topBtnAnimation = CreateSlideAnimation();
				topBtnAnimation.From = topCtrlBtn.Margin;
				topBtnAnimation.To = TopHideMargin;
				Storyboard.SetTarget(topBtnAnimation, topCtrlBtn);
				Storyboard.SetTargetProperty(topBtnAnimation, new PropertyPath(Button.MarginProperty));

				ThicknessAnimation bottomBtnAnimation = CreateSlideAnimation();
				bottomBtnAnimation.From = bottomCtrlBtn.Margin;
				bottomBtnAnimation.To = BottomHideMargin;
				Storyboard.SetTarget(bottomBtnAnimation, bottomCtrlBtn);
				Storyboard.SetTargetProperty(bottomBtnAnimation, new PropertyPath(Button.MarginProperty));

				Storyboard storyboard = new Storyboard();
				storyboard.Children.Add(topBtnAnimation);
				storyboard.Children.Add(bottomBtnAnimation);
				storyboard.Begin(this);
			}
		}

		private const double SlideDurationSec = 0.3;
		/// <summary>
		/// create animation for slide
		/// </summary>
		/// <returns></returns>
		private ThicknessAnimation CreateSlideAnimation()
		{
			ThicknessAnimation marginAnimation = new ThicknessAnimation();
			marginAnimation.Duration = TimeSpan.FromSeconds(SlideDurationSec);

			return marginAnimation;
		}

		private void LeftCtrlBtn_Click(object sender, RoutedEventArgs e)
		{
			// orientation should be horizontal
			ScrollToPreviousItem();
		}

		private void TopCtrlBtn_Click(object sender, RoutedEventArgs e)
		{
			// orientation should be vertical
			ScrollToPreviousItem();
		}

		private void RightCtrlBtn_Click(object sender, RoutedEventArgs e)
		{
			// orientation should be horizontal
			ScrollToNextItem();
		}

		private void BottomCtrlBtn_Click(object sender, RoutedEventArgs e)
		{
			// orientation should be vertical
			ScrollToNextItem();
		}

		#endregion

		#region event handler for user-control

		/// <summary>
		/// re-calculate  offset, re-hide control buttons when ActualHeight or ActualWidth changed,
		/// stop or start dispatcher timer when IsMouseOver changed
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == UserControl.ActualHeightProperty
				|| e.Property == UserControl.ActualWidthProperty)
			{
				// re-set status when ActualHeight or ActualWidth changed
				// TODO: a better handle for ActualWidth and ActualHeight changed when scroll animation is runing
				Reset();
				// re-hide control buttons when ActualHeight or ActualWidth changed
				HideControlButtons();
			}

			if (e.Property == IsMouseOverProperty)
			{
				if (IsMouseOver)
				{
					// Console.WriteLine("stop scroll timer");
					_scrollTimer.Stop();
					SlideShowControlButtons();
				}
				else
				{
					// Console.WriteLine("start scroll timer");
					_scrollTimer.Start();
					SlideHideControlButtons();
				}
			}

			base.OnPropertyChanged(e);
		}

		/// <summary>
		/// disable scroll viewer mouse wheel event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;
		}

		/// <summary>
		/// start dispatcher timer when loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			// initialize Offset
			Offset = OffsetStep * _currentIndex;
			// change ItemsPanel for itemHost
			if (Orientation == Orientation.Horizontal
				&& Resources["horizontalPanel"] is ItemsPanelTemplate hpanel)
			{
				itemsHost.ItemsPanel = hpanel;
			}
			else if (Orientation == Orientation.Vertical
				&& Resources["verticalPanel"] is ItemsPanelTemplate vpanel)
			{
				itemsHost.ItemsPanel = vpanel;
			}
			// change style for overallBar
			// change ItemsPanel for itemHost
			if (Orientation == Orientation.Horizontal
				&& Resources["horizontalListBox"] is Style hstyle)
			{
				overallBar.Style = hstyle;
			}
			else if (Orientation == Orientation.Vertical
				&& Resources["verticalListBox"] is Style vstyle)
			{
				overallBar.Style = vstyle;
			}
			// hide ctrl buttons
			HideControlButtons();
			// start scroll timer
			_scrollTimer.Interval = TimeSpan.FromSeconds(ScrollIntervalSec);
			_scrollTimer.Tick += ScrollTimer_Tick;
			_scrollTimer.Start();
		}

		/// <summary>
		/// reset scrolling status
		/// </summary>
		private void Reset()
		{
			object[] items = Items;
			if (items == null || items.Length == 0)
			{
				_currentIndex = 0;
			}
			else
			{
				_currentIndex = 1;
			}

			_isScrolling = false;
			// remove effection of animation
			this.BeginAnimation(OffsetProperty, null);
			Offset = OffsetStep * _currentIndex;
			// there is a expanded item, so we need minus one
			overallBar.SelectedIndex = _currentIndex - 1;
		}

		#endregion

		private void OverallBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
	}
}