using ImGuiNET;
using System.IO;
using System.Windows.Forms;
using static ImGuiNET.ImGui;

namespace HatsPlusPlusEditor;

class Tab
{
    public string Name { get; set; }
    public Hat? Hat { get; set; }
    public HatElement? CurrentHatElement { get; set; }
    public string? HatPath { get; set; }

    public bool isOpened = true;

    public Tab(string name, Hat? hat = null)
    {
        Name = name;
        Hat = hat;
    }
}

class Editor
{
    private Window _window = null!;
    private Tab? _selectedTab;
    private Tab? _tabToBeSelected = null;
    private int _lastTabID;
    private readonly List<Tab> _tabs = new();
    private bool _elementsMenuOpened = false;
    private Dictionary<HatType, string> _hatTypeNames = new()
    {
        { HatType.Preview, "Preview" },
        { HatType.ExtraHat, "Extra Hat" },
        { HatType.FlyingPet, "Flying Pet" },
        { HatType.Room, "Room"},
        { HatType.WalkingPet, "Walking Pet" },
        { HatType.WereableHat, "Wereable Hat" },
        { HatType.Wings, "Wings" }
    };
    private AnimationWindow _animationWindow = new();
    private HatElement? _lastSelectedElement;

    public void Init(Window window)
    {
        _window = window;

        _animationWindow.Init();
        AddNewEmptyTab();
    }

    public void Update()
    {
        //  ShowDemoWindow();

        SetNextWindowSize(_window.Size.ToSystem());
        SetNextWindowPos(SystemVector2.Zero);

        var mainWindowFlags =
            ImGuiWindowFlags.MenuBar
            | ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoBringToFrontOnFocus
            | ImGuiWindowFlags.NoBackground
            | ImGuiWindowFlags.NoResize;

        if (Begin("Editor", mainWindowFlags))
        {
            ShowEditor();
            End();
        }

        ShowAnimationWindow();

        _lastSelectedElement = _selectedTab?.CurrentHatElement!;
    }

    private void ShowAnimationWindow()
    {
        if (_lastSelectedElement != _selectedTab?.CurrentHatElement)
        {
            _animationWindow.TrySetHatElement(_selectedTab?.CurrentHatElement);
        }

        _animationWindow.Update();
    }

    private void ShowEditor()
    {
        if (BeginMenuBar())
        {
            ShowMenuBar();
            EndMenuBar();
        }
        if (BeginTabBar("Tabs", ImGuiTabBarFlags.Reorderable))
        {
            ShowTabBar();
            EndTabBar();
        }

        if (_selectedTab == null)
            return;

        var hat = _selectedTab.Hat;
        if (hat == null)
        {
            Text("It's so empty...");
            return;
        }

        var currentElement = _selectedTab.CurrentHatElement;
        if (currentElement == null)
        {
            Text("Select or add an element :)");
            return;
        }

        ImguiUtils.TextVerticallyAlignedWithButtons("Current path:");
        SetCursorPosX(GetCursorPosX() - GetStyle().FramePadding.X);
        if (ImguiUtils.TextLinkVerticallyAlignedWithButtons(currentElement.texture.Path, !_elementsMenuOpened) && !_elementsMenuOpened)
        {
            var folderPath = new FileInfo(currentElement.texture.Path).Directory!.FullName;

            if (folderPath != null)
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }
        if (Button("Set path"))
        {
            if (TryAddHatElement(currentElement.type, hat))
                currentElement.texture.Delete();

            //TODO: update animation window
        }

        if (Button("Remove Element"))
        {
            RemoveHatElement(currentElement, hat);
            _selectedTab.CurrentHatElement = null;
        }
    }

    private Tab AddNewEmptyTab(string? name = null)
    {
        name ??= $"Hat {_lastTabID++}";

        var tab = new Tab(name);
        tab.Hat = new Hat();

        _tabs.Add(tab);

        return tab;
    }

    private void RemoveTab(Tab tab)
    {
        if (tab == _selectedTab)
        {
            var index = _tabs.IndexOf(tab);

            _tabs.Remove(tab);

            if (_tabs.Count == 0)
                return;
            if (index == _tabs.Count)
                index -= 1;

            _selectedTab = _tabs[index];
            _tabToBeSelected = _tabs[index];

            return;
        }

        _tabs.Remove(tab);
    }

    private void ShowTabBar()
    {
        Tab? tabToRemove = null;

        foreach (var tab in _tabs)
        {
            var name = tab.Name;
            var flags = _tabToBeSelected != null && tab == _tabToBeSelected
                ? ImGuiTabItemFlags.SetSelected : 0;

            if (BeginTabItem(name, ref tab.isOpened, flags))
            {
                _selectedTab = tab;

                EndTabItem();
            }

            if (!tab.isOpened)
                tabToRemove = tab;
        }

        if (_tabToBeSelected != null)
            _tabToBeSelected = null;

        if (tabToRemove != null)
            RemoveTab(tabToRemove);

        var plusFlags =
            ImGuiTabItemFlags.Trailing
            | ImGuiTabItemFlags.NoTooltip;
        if (TabItemButton("+", plusFlags))
        {
            AddNewEmptyTab();
        }
    }

    private void ShowMenuBar()
    {
        if (BeginMenu("Menu"))
        {
            ShowMenu();
            EndMenu();
        }
        if (BeginMenu("Elements"))
        {
            _elementsMenuOpened = true;
            ShowElements();
            EndMenu();
        }
        else _elementsMenuOpened = false;
    }

    private void ShowElements()
    {
        if (_selectedTab == null)
            return;

        var tab = _selectedTab;
        var currentHat = tab.Hat;

        if (currentHat == null)
            return;

        if (currentHat.previewHat != null && MenuItem(_hatTypeNames[HatType.Preview]))
        {
            tab.CurrentHatElement = currentHat.previewHat;
        }
        if (currentHat.wereableHat != null && MenuItem(_hatTypeNames[HatType.WereableHat]))
        {
            // _animationWindow.SetHatElement(currentHat.wereableHat);
            tab.CurrentHatElement = currentHat.wereableHat;
        }
        if (currentHat.wingsHat != null && MenuItem(_hatTypeNames[HatType.Wings]))
        {
            //_animationWindow.SetHatElement(currentHat.wingsHat);
            tab.CurrentHatElement = currentHat.wingsHat;
        }
        if (currentHat.extraHat != null && MenuItem(_hatTypeNames[HatType.ExtraHat]))
        {
            //_animationWindow.SetHatElement(currentHat.extraHat);
            tab.CurrentHatElement = currentHat.extraHat;
        }

        int flyingPetNumber = 0, walkingPetNumber = 0;
        foreach (var pet in currentHat.pets)
        {
            var petName = _hatTypeNames[pet.type];

            switch (pet.type)
            {
                case HatType.FlyingPet:
                    flyingPetNumber++;
                    petName += $" {flyingPetNumber}";
                    break;
                case HatType.WalkingPet:
                    walkingPetNumber++;
                    petName += $" {walkingPetNumber}";
                    break;
            }

            if (MenuItem(petName))
            {
                tab.CurrentHatElement = pet;
                // _animationWindow.SetHatElement(pet);
            }
        }

        if (BeginMenu("Add New Element"))
        {
            if (currentHat.previewHat == null && MenuItem(_hatTypeNames[HatType.Preview]))
            {
                TryAddHatElement(HatType.Preview, currentHat);
            }
            if (currentHat.wereableHat == null && MenuItem(_hatTypeNames[HatType.WereableHat]))
            {
                TryAddHatElement(HatType.WereableHat, currentHat);
            }
            if (currentHat.wingsHat == null && MenuItem(_hatTypeNames[HatType.Wings]))
            {
                TryAddHatElement(HatType.Wings, currentHat);
            }
            if (currentHat.extraHat == null && MenuItem(_hatTypeNames[HatType.ExtraHat]))
            {
                TryAddHatElement(HatType.ExtraHat, currentHat);
            }

            if (currentHat.pets.Count == 5)
            {
                EndMenu();
                return;
            }

            if (MenuItem(_hatTypeNames[HatType.FlyingPet]))
            {
                TryAddHatElement(HatType.FlyingPet, currentHat);
            }
            if (MenuItem(_hatTypeNames[HatType.WalkingPet]))
            {
                TryAddHatElement(HatType.WalkingPet, currentHat);
            }

            EndMenu();
        }
    }

    private bool TryAddHatElement(HatType type, Hat hat)
    {
        using var dialog = new OpenFileDialog();
        if (dialog.ShowDialog() != DialogResult.OK)
            return false;

        var path = dialog.FileName;

        if (Path.GetExtension(path) != ".png")
        {
            //TODO: display error window
            return false;
        }

        switch (type)
        {
            case HatType.WalkingPet:
                var walkingPet = HatLoader.LoadWalkingPet(path);
                hat.pets.Add(walkingPet);
                _selectedTab!.CurrentHatElement = walkingPet;
                break;
            case HatType.FlyingPet:
                var flyingPet = HatLoader.LoadFlyingPet(path);
                hat.pets.Add(flyingPet);
                _selectedTab!.CurrentHatElement = flyingPet;
                break;
            case HatType.Preview:
                var preview = HatLoader.LoadPreview(path);
                hat.previewHat = preview;
                _selectedTab!.CurrentHatElement = preview;
                break;
            case HatType.ExtraHat:
                var extraHat = HatLoader.LoadExtraHat(path);
                hat.extraHat = extraHat;
                _selectedTab!.CurrentHatElement = extraHat;
                break;
            case HatType.WereableHat:
                var wereableHat = HatLoader.LoadWereableHat(path);
                hat.wereableHat = wereableHat;
                _selectedTab!.CurrentHatElement = wereableHat;
                break;
            case HatType.Wings:
                var wings = HatLoader.LoadWings(path);
                hat.wingsHat = wings;
                _selectedTab!.CurrentHatElement = wings;
                break;
            case HatType.Room:
                //TODO: add room loading
                break;
        }

        return true;
    }

    private void RemoveHatElement(HatElement element, Hat hat)
    {
        element.texture.Delete();

        switch (element.type)
        {
            case HatType.WalkingPet:
                hat.pets.Remove((element as WalkingPetHat)!);
                break;
            case HatType.FlyingPet:
                hat.pets.Remove((element as FlyingPetHat)!);
                break;
            case HatType.Preview:
                hat.previewHat = null;
                break;
            case HatType.ExtraHat:
                hat.extraHat = null;
                break;
            case HatType.WereableHat:
                hat.wereableHat = null;
                break;
            case HatType.Wings:
                hat.wingsHat = null;
                break;
            case HatType.Room:
                hat.roomHat = null;
                break;
        }
    }

    private void ShowMenu()
    {
        if (MenuItem("New"))
        {
            _tabToBeSelected = AddNewEmptyTab();
        }
        if (MenuItem("Open"))
        {
            using var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var hat = HatLoader.LoadHat(dialog.SelectedPath);
                var hatName = Path.GetFileNameWithoutExtension(dialog.SelectedPath);
                if (_tabs.Exists(tab => tab.Name == hatName))
                {

                }
                //TODO: check if there's already a hat with the same name
                Tab tab = new Tab(hatName, hat);
                _tabs.Add(tab);
                
                _tabToBeSelected = tab;

            }
        }

        if (_selectedTab == null || _selectedTab.Hat == null)
            return;

        if (MenuItem("Save"))
        {
            if (_selectedTab.HatPath == null)
                _selectedTab.HatPath = SaveHatAs(_selectedTab.Hat);
            else SaveHat(_selectedTab.Hat, _selectedTab.HatPath);
        }
        if (MenuItem("Save as"))
        {
            _selectedTab.HatPath = SaveHatAs(_selectedTab.Hat);
        }
    }

    private void SaveHat(Hat hat, string folderPath)
    {
        if (hat.previewHat != null)
            HatSaver.SavePreviewHat(hat.previewHat, folderPath);
    }

    private string? SaveHatAs(Hat hat)
    {
        using var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            SaveHat(hat, dialog.SelectedPath);
            return dialog.SelectedPath;
        }

        return null;
    }
}