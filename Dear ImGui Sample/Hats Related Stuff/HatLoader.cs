using System.IO;
using System.Linq;

namespace HatsPlusPlusEditor;

public static class HatLoader
{
    public static Hat LoadHat(string path)
    {
        var dirInfo = new DirectoryInfo(path);
        var customHat = new Hat(path);

        foreach (var file in dirInfo.EnumerateFiles())
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FullName);

            if (fileName.ToLower() == "preview")
            {
                customHat.previewHat = LoadPreviewHatInternal(file.FullName);
                continue;
            }
            else if (fileName.ToLower() == "extrahat")
            {
                customHat.extraHat = LoadExtraHatInternal(file.FullName, Vector2i.Zero);
                continue;
            }

            if (!GetHatNameData(fileName, out var hatName, out var size))
                continue;

            switch (hatName.ToLower())
            {
                case "hat":
                    customHat.wereableHat = LoadWereableHatInternal(file.FullName, size);
                    break;
                case "extrahat":
                    customHat.extraHat = LoadExtraHatInternal(file.FullName, size);
                    break;
                case "room":
                    //To be finished
                    break;
                case "walkingpet":
                    customHat.pets ??= new();
                    customHat.pets.Add(LoadWalkingPetInternal(file.FullName, size));
                    break;
                case "flyingpet":
                    customHat.pets ??= new();
                    customHat.pets.Add(LoadFlyingPetInternal(file.FullName, size));
                    break;
                case "wings":
                    customHat.wingsHat = LoadWingsInternal(file.FullName, size);
                    break;
            }
        }

        return customHat;
    }

    public static WingsHat LoadWings(string path)
    {
        var parentFolderPath = new FileInfo(path).Directory!.FullName;

        if (parentFolderPath != null && GetHatNameData(parentFolderPath, out var hatName, out var size))
            return LoadWingsInternal(path, size);

        return LoadWingsInternal(path);
    }

    public static FlyingPetHat LoadFlyingPet(string path)
    {
        var parentFolderPath = new FileInfo(path).Directory!.FullName;

        if (parentFolderPath != null && GetHatNameData(parentFolderPath, out var hatName, out var size))
            return LoadFlyingPetInternal(path, size);

        return LoadFlyingPetInternal(path);
    }

    public static WalkingPetHat LoadWalkingPet(string path)
    {
        var parentFolderPath = new FileInfo(path).Directory!.FullName;

        if (parentFolderPath != null && GetHatNameData(parentFolderPath, out var hatName, out var size))
            return LoadWalkingPetInternal(path, size);

        return LoadWalkingPetInternal(path);
    }

    public static WereableHat LoadWereableHat(string path)
    {
        var parentFolderPath = new FileInfo(path).Directory!.FullName;

        if (parentFolderPath != null && GetHatNameData(parentFolderPath, out var hatName, out var size))
            return LoadWereableHatInternal(path, size);

        return LoadWereableHatInternal(path);
    }

    public static PreviewHat LoadPreview(string path)
    {
        return LoadPreviewHatInternal(path);
    }

    public static ExtraHat LoadExtraHat(string path)
    {
        var parentFolderPath = new FileInfo(path).Directory!.FullName;

        if (parentFolderPath != null && GetHatNameData(parentFolderPath, out var hatName, out var size))
            return LoadExtraHatInternal(path, size);

        return LoadExtraHatInternal(path);
    }

    private static WingsHat LoadWingsInternal(string path, Vector2i size = new Vector2i())
    {
        var texture = Texture.Load(path);
        var bitmap = MyBitmap.Create(texture);
        size = size == Vector2i.Zero ? texture.Size : size;

        WingsHat hat = new()
        {
            texture = texture,
            type = HatType.Wings,
            hatAreaSize = size
        };

        var metapixels = GetMetapixels(bitmap, size);

        for (int i = 0; i < metapixels.Count; i++)
        {
            var pixel = metapixels[i];

            switch ((MetapixelType)pixel.R)
            {
                default:
                    break;
                case MetapixelType.WingsGeneralOffset:
                    hat.generalOffset = new Vector2i(pixel.G - 128, pixel.B - 128);
                    break;
                case MetapixelType.WingsCrouchOffset:
                    hat.crouchOffset = new Vector2i(pixel.G - 128, pixel.B - 128);
                    break;
                case MetapixelType.WingsRagdollOffset:
                    hat.ragdollOffset = new Vector2i(pixel.G - 128, pixel.B - 128);
                    break;
                case MetapixelType.WingsSlideOffset:
                    hat.slideOffset = new Vector2i(pixel.G - 128, pixel.B - 128);
                    break;
                case MetapixelType.GenerateWingsAnimations:
                    hat.generateAnimations = true;
                    break;
                case MetapixelType.WingsAutoGlideFrame:
                    hat.autoAnimationsSpeed = pixel.G;
                    break;
                case MetapixelType.WingsAutoIdleFrame:
                    hat.autoIdleFrame = pixel.G;
                    break;
                case MetapixelType.WingsAutoAnimationsSpeed:
                    hat.autoAnimationsSpeed = pixel.G;
                    break;
                case MetapixelType.ChangeAnimationsEveryLevel:
                    hat.changeAnimationEveryLevel = true;
                    break;
                case MetapixelType.WingsNetOffset:
                    hat.netOffset = new Vector2i(pixel.G - 128, pixel.B - 128);
                    break;
                case MetapixelType.IsBigHat:
                    hat.isBigHat = true;
                    break;
                case MetapixelType.FrameSize:
                    hat.frameSize = new Vector2i(pixel.G, pixel.B);
                    break;
            }
        }

        return hat;
    }

    private static FlyingPetHat LoadFlyingPetInternal(string path, Vector2i size = new Vector2i())
    {
        var texture = Texture.Load(path);
        var bitmap = MyBitmap.Create(texture);
        size = size == Vector2i.Zero ? texture.Size : size;

        FlyingPetHat hat = new()
        {
            texture = texture,
            type = HatType.FlyingPet,
            hatAreaSize = size
        };

        var metapixels = GetMetapixels(bitmap, size);

        for (int i = 0; i < metapixels.Count; i++)
        {
            var pixel = metapixels[i];

            switch ((MetapixelType)pixel.R)
            {
                default:
                    break;
                case MetapixelType.PetDistance:
                    hat.distance = pixel.G;
                    break;
                case MetapixelType.PetNoFlip:
                    hat.noFlip = true;
                    break;
                case MetapixelType.PetChangeAngle:
                    hat.changesAngle = true;
                    break;
                case MetapixelType.PetSpeed:
                    hat.speed = pixel.G;
                    break;
                case MetapixelType.FrameSize:
                    hat.frameSize = new Vector2i(pixel.G, pixel.B);
                    break;
                case MetapixelType.AnimationType:
                    hat.animations.Add(GetAnimation(metapixels, i, out var skipAmount));
                    i += skipAmount;
                    break;
                case MetapixelType.IsBigHat:
                    hat.isBigHat = true;
                    break;
            }
        }

        if (hat.frameSize == Vector2i.Zero)
            hat.frameSize = hat.texture.Size;

        return hat;
    }

    private static WalkingPetHat LoadWalkingPetInternal(string path, Vector2i size = new Vector2i())
    {
        var texture = Texture.Load(path);
        var bitmap = MyBitmap.Create(texture);
        size = size == Vector2i.Zero ? texture.Size : size;

        WalkingPetHat hat = new()
        {
            texture = texture,
            type = HatType.WalkingPet,
            hatAreaSize = size
        };

        var metapixels = GetMetapixels(bitmap, size);

        for (int i = 0; i < metapixels.Count; i++)
        {
            var pixel = metapixels[i];

            switch ((MetapixelType)pixel.R)
            {
                default:
                    break;
                case MetapixelType.PetDistance:
                    hat.distance = pixel.G;
                    break;
                case MetapixelType.PetNoFlip:
                    hat.noFlip = true;
                    break;
                case MetapixelType.FrameSize:
                    hat.frameSize = new Vector2i(pixel.G, pixel.B);
                    break;
                case MetapixelType.AnimationType:
                    hat.animations.Add(GetAnimation(metapixels, i, out var skipAmount));
                    i += skipAmount;
                    break;
                case MetapixelType.IsBigHat:
                    hat.isBigHat = true;
                    break;
            }
        }

        if (hat.frameSize == Vector2i.Zero)
            hat.frameSize = hat.texture.Size;

        return hat;
    }

    private static ExtraHat LoadExtraHatInternal(string path, Vector2i size = new Vector2i())
    {
        var texture = Texture.Load(path);
        var bitmap = MyBitmap.Create(texture);
        size = size == Vector2i.Zero ? texture.Size : size;

        ExtraHat hat = new()
        {
            texture = texture,
            type = HatType.ExtraHat,
            hatAreaSize = size
        };

        if (size.EuclideanLength == 0)
            return hat;

        var metapixels = GetMetapixels(bitmap, size);

        for (int i = 0; i < metapixels.Count; i++)
        {
            var pixel = metapixels[i];

            switch ((MetapixelType)pixel.R)
            {
                default:
                    break;
                case MetapixelType.FrameSize:
                    hat.frameSize = new Vector2i(pixel.G, pixel.B);
                    break;
            }
        }

        return hat;
    }

    private static WereableHat LoadWereableHatInternal(string path, Vector2i size = new Vector2i())
    {
        var texture = Texture.Load(path);
        var bitmap = MyBitmap.Create(texture);
        size = size == Vector2i.Zero ? texture.Size : size;

        WereableHat hat = new()
        {
            texture = texture,
            type = HatType.WereableHat,
            hatAreaSize = size
        };

        var metapixels = GetMetapixels(bitmap, size);

        for (int i = 0; i < metapixels.Count; i++)
        {
            var pixel = metapixels[i];

            switch ((MetapixelType)pixel.R)
            {
                default:
                    break;
                case MetapixelType.StrappedOn:
                    hat.isStrappedOn = true;
                    break;
                case MetapixelType.IsBigHat:
                    hat.isBigHat = true;
                    break;
                case MetapixelType.FrameSize:
                    hat.frameSize = new Vector2i(pixel.G, pixel.B);
                    break;
                case MetapixelType.QuackAnimationsLinked:
                    hat.quackLinkState = (LinkQuackAnimationsState)int.Min(2, pixel.G);
                    break;
                case MetapixelType.OnSpawnAnimation:
                    hat.onSpawnAnimation = pixel.G;
                    break;
                case MetapixelType.AnimationType:
                    hat.animations.Add(GetAnimation(metapixels, i, out var skipAmount));
                    i += skipAmount;
                    break;
            }
        }

        return hat;
    }

    private static HatAnimation GetAnimation(List<Metapixel> metapixels, int index, out int skipAmount)
    {
        skipAmount = 3;
        var animationType = metapixels[index];
        var delay = metapixels[index + 1];
        var loop = metapixels[index + 2];
        var frameOrRange = metapixels[index + 3];
        var type = (HatAnimationType)animationType.G;

        if (frameOrRange.Type == MetapixelType.AnimationFramePeriod)
        {
            var frames = Enumerable.Range(frameOrRange.G, frameOrRange.B - frameOrRange.G + 1).ToList();
            return new(type, delay.G, loop.G == 1, frames);
        }

        var framesList = new List<int>() { frameOrRange.G };

        while (true)
        {
            var frameIndex = index + skipAmount + 1;
            if (frameIndex == metapixels.Count)
                break;

            var frame = metapixels[frameIndex];

            if (frame.Type != MetapixelType.AnimationFrame)
                break;

            framesList.Add(frame.G);
            skipAmount++;
        }

        return new(type, delay.G, loop.G == 1, framesList);
    }

    private static PreviewHat LoadPreviewHatInternal(string path)
    {
        var texture = Texture.Load(path);
        return new PreviewHat(texture) { type = HatType.Preview };
    }

    private static List<Metapixel> GetMetapixels(MyBitmap bitmap, Vector2i size)
    {
        Vector2i metapixelsAreaSize = new(bitmap.Width - size.X, bitmap.Height);
        List<Metapixel> metapixels = new();

        for (int x = 0; x < metapixelsAreaSize.X; x++)
        {
            for (int y = 0; y < metapixelsAreaSize.Y; y++)
            {
                var pixel = bitmap.GetPixel(new Vector2i(size.X + x, y));

                if (pixel.IsEmpty())
                    continue;

                metapixels.Add(new((int)pixel.R, pixel.G, pixel.B));
            }
        }

        return metapixels;
    }

    private static bool GetHatNameData(string fullName, out string name, out Vector2i size)
    {
        if (fullName.IndexOf("_") is int firstIndex && firstIndex == -1)
        {
            size = new Vector2i(-1);
            name = fullName;
            return false;
        }

        size = Vector2i.Zero;
        var sizeString = fullName.Substring(firstIndex + 1);
        var sizes = sizeString.Split("_");
        size.X = Convert.ToInt32(sizes[0]);
        size.Y = Convert.ToInt32(sizes[1]);

        name = fullName.Substring(0, firstIndex);
        return true;
    }
}