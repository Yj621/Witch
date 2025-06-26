using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TinyGiantStudio.DevTools
{
    public class Header
    {
        private DevToolsWindow devToolsWindow;

        private readonly string companyAssetStoreLink = "https://assetstore.unity.com/publishers/45848?aid=1011ljxWe";
        private readonly string assetLink = "https://assetstore.unity.com/packages/slug/291626?aid=1011ljxWe";

        private GroupBox assetInformationGroupBox;

        public ToolbarSearchField searchBar;

        private Button companyButton;

        private List<VisualElement> icons = new();

        public Header(VisualElement root, DevToolsWindow devToolsWindow)
        {
            Setup(root, devToolsWindow);
        }

        /// <summary>
        /// Called once when this script instance is created
        /// </summary>
        /// <param name="groupBox"></param>
        /// <param name="devToolsWindow"></param>
        private void Setup(VisualElement root, DevToolsWindow devToolsWindow)
        {
            var groupBox = root.Q<GroupBox>("HeaderRoot");
            this.devToolsWindow = devToolsWindow;

            SetupAssetInformations(groupBox);

            SetupTabs(groupBox, devToolsWindow);

            searchBar = groupBox.Q<ToolbarSearchField>("SearchBar");
            string originalSearchingFor = devToolsWindow.SearchManger.SearchingFor();
            searchBar.value = originalSearchingFor;
            searchBar.RegisterValueChangedCallback((evt) =>
            {
                devToolsWindow.SearchManger.SetNewSearch(evt.newValue);
                devToolsWindow.UpdatePages();
            });

            groupBox.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                Adapt(evt.newRect.width);
            });
        }

        private void SetupAssetInformations(GroupBox groupBox)
        {
            assetInformationGroupBox = groupBox.Q<GroupBox>("AssetInformation");

            var AssetIconButton = groupBox.Q<Button>("AssetIconButton");
            AssetIconButton.clicked += () =>
            {
                Application.OpenURL(assetLink);
            };
            var AssetNameButton = groupBox.Q<Button>("AssetNameButton");
            AssetNameButton.clicked += () =>
            {
                Application.OpenURL(assetLink);
            };

            companyButton = groupBox.Q<Button>("CompanyButton");
            companyButton.clicked += () =>
            {
                Application.OpenURL(companyAssetStoreLink);
            };
        }

        public List<Label> tabNames = new();

        public void SetupTabs(GroupBox groupBox, DevToolsWindow devToolsWindow)
        {
            VisualTreeAsset tabAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/Tiny Giant Studio/DevTools/Common/Scripts/Editor/Header/TabTemplate.uxml");

            GroupBox tabHolder = groupBox.Q<GroupBox>("Tabs");
            tabHolder.Clear(); //Just in-case

            tabNames ??= new();
            tabNames.Clear();

            List<TabPage> pages = devToolsWindow.tabPages;
            for (int i = 0; i < pages.Count; i++)
            {
                TabPage page = pages[i];
                if (page == null)
                    continue;

                page.ClosePage();

                VisualElement tab = new();
                tabAsset.CloneTree(tab);
                tabHolder.Add(tab);
                page.tab = tab;

                tab.tooltip = page.tooltip;

                var nameLabel = tab.Q<Label>("Name");
                nameLabel.text = page.tabName;
                tabNames.Add(nameLabel);
                var icon = tab.Q<VisualElement>("Icon");
                icon.AddToClassList(page.tabIcon);
                icons.Add(icon);

                var b = tab.Q<Button>();
                b.clicked += () => { SelectTab(page); };
            }
        }

        /// <summary>
        /// Selects the tab page and saves it in settings.
        ///
        /// The alternative method is ResumeTab that doesn't save selected tab
        /// </summary>
        /// <param name="selectedPage"></param>
        public void SelectTab(TabPage selectedPage)
        {
            Settings.instance.selectedTab = selectedPage;
            Settings.instance.selectedPage = null;

            SelectTabBase(selectedPage);
        }

        /// <summary>
        /// Select Tab without changing selected tab, page in settings
        ///
        /// SelectTab method does.
        /// </summary>
        /// <param name="selectedPage"></param>
        public void ResumeTab(TabPage selectedPage)
        {
            SelectTabBase(selectedPage);
        }

        private void SelectTabBase(TabPage selectedPage)
        {
            selectedPage.UpdatePage();
            selectedPage.OpenPage();
            selectedPage.tab.AddToClassList("tab-selected");

            foreach (TabPage page in devToolsWindow.tabPages)
            {
                if (page != selectedPage)
                {
                    page.ClosePage();
                    page.tab?.RemoveFromClassList("tab-selected");
                }
            }
        }

        private void Adapt(float width)
        {
            if (width < 350)
            {
                HideLabels();
                ShowIcons();
                assetInformationGroupBox.style.display = DisplayStyle.None;
            }
            else if (width < 500)
            {
                HideFullTabLabels();
                HideIcons();
                assetInformationGroupBox.style.display = DisplayStyle.None;
            }
            else if (width < 600)
            {
                ShowFullTabLabels();
                HideIcons();
                assetInformationGroupBox.style.display = DisplayStyle.None;
            }
            else if (width < 710)
            {
                ShowFullTabLabels();
                HideIcons();
                assetInformationGroupBox.style.display = DisplayStyle.None;
            }
            else if (width < 815)
            {
                ShowFullTabLabels();
                HideIcons();
                assetInformationGroupBox.style.display = DisplayStyle.Flex;
            }
            else if (width < 880)
            {
                ShowFullTabLabels();
                companyButton.text = "Tiny Giant Studio";
                ShowIcons();
                assetInformationGroupBox.style.display = DisplayStyle.Flex;
            }
            else
            {
                ShowFullTabLabels();
                companyButton.text = "by Tiny Giant Studio";
                ShowIcons();
                assetInformationGroupBox.style.display = DisplayStyle.Flex;
            }
        }

        private void ShowFullTabLabels()
        {
            List<TabPage> tabPages = devToolsWindow.tabPages;

            if (tabPages.Count != tabNames.Count)
                return;

            for (int i = 0; i < tabNames.Count; i++)
            {
                tabNames[i].style.display = DisplayStyle.Flex;
                tabNames[i].text = tabPages[i].tabName;
            }
        }

        private void HideFullTabLabels()
        {
            List<TabPage> tabPages = devToolsWindow.tabPages;

            if (tabPages.Count != tabNames.Count)
                return;

            for (int i = 0; i < tabNames.Count; i++)
            {
                tabNames[i].style.display = DisplayStyle.Flex;
                tabNames[i].text = tabPages[i].tabShortName;
            }
        }

        private void HideLabels()
        {
            List<TabPage> tabPages = devToolsWindow.tabPages;

            if (tabPages.Count != tabNames.Count)
                return;

            for (int i = 0; i < tabNames.Count; i++)
            {
                tabNames[i].style.display = DisplayStyle.None;
            }
        }

        private void ShowIcons()
        {
            foreach (VisualElement icon in icons)
            {
                icon.style.display = DisplayStyle.Flex;
            }
        }

        private void HideIcons()
        {
            foreach (VisualElement icon in icons)
            {
                icon.style.display = DisplayStyle.None;
            }
        }
    }
}