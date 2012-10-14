﻿

//------------------------------------------------------------------------------
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//------------------------------------------------------------------------------
#include "pch.h"
#include "GroupedItemsPage.xaml.h"




void ::BrainGraph::GroupedItemsPage::InitializeComponent()
{
    if (_contentLoaded)
        return;

    _contentLoaded = true;

    // Call LoadComponent on ms-appx:///GroupedItemsPage.xaml
    ::Windows::UI::Xaml::Application::LoadComponent(this, ref new ::Windows::Foundation::Uri(L"ms-appx:///GroupedItemsPage.xaml"), ::Windows::UI::Xaml::Controls::Primitives::ComponentResourceLocation::Application);

    // Get the CollectionViewSource named 'groupedItemsViewSource'
    groupedItemsViewSource = safe_cast<::Windows::UI::Xaml::Data::CollectionViewSource^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"groupedItemsViewSource"));
    // Get the GridView named 'itemGridView'
    itemGridView = safe_cast<::Windows::UI::Xaml::Controls::GridView^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"itemGridView"));
    // Get the ListView named 'itemListView'
    itemListView = safe_cast<::Windows::UI::Xaml::Controls::ListView^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"itemListView"));
    // Get the Button named 'backButton'
    backButton = safe_cast<::Windows::UI::Xaml::Controls::Button^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"backButton"));
    // Get the TextBlock named 'pageTitle'
    pageTitle = safe_cast<::Windows::UI::Xaml::Controls::TextBlock^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"pageTitle"));
    // Get the VisualStateGroup named 'ApplicationViewStates'
    ApplicationViewStates = safe_cast<::Windows::UI::Xaml::VisualStateGroup^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"ApplicationViewStates"));
    // Get the VisualState named 'FullScreenLandscape'
    FullScreenLandscape = safe_cast<::Windows::UI::Xaml::VisualState^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"FullScreenLandscape"));
    // Get the VisualState named 'Filled'
    Filled = safe_cast<::Windows::UI::Xaml::VisualState^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"Filled"));
    // Get the VisualState named 'FullScreenPortrait'
    FullScreenPortrait = safe_cast<::Windows::UI::Xaml::VisualState^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"FullScreenPortrait"));
    // Get the VisualState named 'Snapped'
    Snapped = safe_cast<::Windows::UI::Xaml::VisualState^>(static_cast<Windows::UI::Xaml::IFrameworkElement^>(this)->FindName(L"Snapped"));
}

void ::BrainGraph::GroupedItemsPage::Connect(int connectionId, Platform::Object^ target)
{
    switch (connectionId)
    {
    case 1:
        (safe_cast<::Windows::UI::Xaml::Controls::ListViewBase^>(target))->ItemClick +=
            ref new ::Windows::UI::Xaml::Controls::ItemClickEventHandler(this, (void (::BrainGraph::GroupedItemsPage::*)(Platform::Object^, Windows::UI::Xaml::Controls::ItemClickEventArgs^))&GroupedItemsPage::ItemView_ItemClick);
        break;
    case 2:
        (safe_cast<::Windows::UI::Xaml::Controls::ListViewBase^>(target))->ItemClick +=
            ref new ::Windows::UI::Xaml::Controls::ItemClickEventHandler(this, (void (::BrainGraph::GroupedItemsPage::*)(Platform::Object^, Windows::UI::Xaml::Controls::ItemClickEventArgs^))&GroupedItemsPage::ItemView_ItemClick);
        break;
    case 3:
        (safe_cast<::Windows::UI::Xaml::Controls::Primitives::ButtonBase^>(target))->Click +=
            ref new ::Windows::UI::Xaml::RoutedEventHandler(this, (void (::BrainGraph::GroupedItemsPage::*)(Platform::Object^, Windows::UI::Xaml::RoutedEventArgs^))&GroupedItemsPage::GoBack);
        break;
    case 4:
        (safe_cast<::Windows::UI::Xaml::Controls::Primitives::ButtonBase^>(target))->Click +=
            ref new ::Windows::UI::Xaml::RoutedEventHandler(this, (void (::BrainGraph::GroupedItemsPage::*)(Platform::Object^, Windows::UI::Xaml::RoutedEventArgs^))&GroupedItemsPage::Header_Click);
        break;
    case 5:
        (safe_cast<::Windows::UI::Xaml::Controls::Primitives::ButtonBase^>(target))->Click +=
            ref new ::Windows::UI::Xaml::RoutedEventHandler(this, (void (::BrainGraph::GroupedItemsPage::*)(Platform::Object^, Windows::UI::Xaml::RoutedEventArgs^))&GroupedItemsPage::Header_Click);
        break;
    }
    (void)connectionId; // Unused parameter
    (void)target; // Unused parameter
    _contentLoaded = true;
}

