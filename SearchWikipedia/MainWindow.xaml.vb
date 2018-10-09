Imports System
Imports System.Reactive
Imports System.Reactive.Linq
Imports System.Runtime.CompilerServices

Module WpfExtensions

    <Extension()>
    Public Function ObserveKeys(textbox As TextBox) As IObservable(Of String)
        Return Observable _
        .FromEventPattern(Of KeyEventArgs)(textbox, "KeyUp") _
        .Select(Function(a) textbox.Text)
        '.DistinctUntilChanged()
    End Function

End Module

Class MainWindow

    Public Sub New()
        MyBase.New()
        InitializeComponent()
        Dim keys = Search.ObserveKeys() _
            .Throttle(TimeSpan.FromSeconds(0.8)) _
            .ObserveOnDispatcher().Subscribe(
            Sub(txt)
                lblSearch.Text = "Searching for..." + txt
                lblProgress.Visibility = Visibility.Visible
                Dim uri = New Uri("http://en.wikipedia.org/wiki/" + txt)
                webBrowser1.Navigate(uri)
            End Sub)

        Dim browser = Observable.FromEventPattern(Of NavigationEventArgs)(webBrowser1, "Navigated")
        browser.ObserveOnDispatcher.Subscribe(Sub(evt) lblProgress.Visibility = Visibility.Collapsed)
    End Sub

End Class
