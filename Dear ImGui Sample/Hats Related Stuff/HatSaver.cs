using System.IO;

namespace HatsPlusPlusEditor;

static class HatSaver
{
    public static void SavePreviewHat(PreviewHat hat, string folderPath)
    {
        hat.texture.SaveRGBA(Path.Combine(folderPath, "preview.png"));
    }

    public static void SaveWings(WingsHat hat, string folderPath)
    {

    }

    public static void SaveWalkingPet(WalkingPetHat hat, string folderPath)
    {

    }

    public static void SaveFlyingPetHat(FlyingPetHat hat, string folderPath)
    {

    }

    public static void SaveExtraHat(ExtraHat hat, string folderPath)
    {

    }

    public static void SaveRoomHat(RoomHat hat, string folderPath)
    {

    }
}