using UnityEngine;

[System.Serializable]
public class Level
{
    [SerializeField]
    private string editorName;
    public string EditorName
    {
        get { return editorName; }
        set { editorName = value; }
    }

    [SerializeField]
    private ItemSave[] items;
    public ItemSave[] Items
    {
        get { return items; }
        set { items = value; }
    }

    [System.Serializable]
    public class ItemSave
    {
        [SerializeField]
        private Item item;
        public Item Item
        {
            get { return item; }
        }

        [SerializeField]
        private LevelItemBehaviour.ItemType type;
        public LevelItemBehaviour.ItemType Type
        {
            get { return type; }
        }

        [SerializeField]
        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
        }

        [SerializeField]
        private Vector3 rotation;
        public Vector3 Rotation
        {
            get { return rotation; }
        }

        [SerializeField]
        private Vector3 scale;
        public Vector3 Scale
        {
            get { return scale; }
        }

        [SerializeField]
        private bool isStatic = false;
        public bool IsStatic
        {
            get { return isStatic; }
        }

        public ItemSave(Item item, LevelItemBehaviour.ItemType type, Vector3 position, Vector3 rotation, Vector3 scale, bool isStatic)
        {
            this.item = item;
            this.type = type;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            this.isStatic = isStatic;
        }
    }

    public enum Item
    {
        Cube = 0,
        Sphere = 1,
        Cylinder = 2,
        Piramide = 3,
    }
}