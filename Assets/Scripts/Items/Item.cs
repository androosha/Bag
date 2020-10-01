using System;

public enum ItemType
{
    Default = 0,
    Sphere = 1,
    Cube = 2,
    Prysm = 3,
}

[Serializable]
public class Item
{
    public int mass;
    public int id;
    public string name;
    public ItemType type;

    new public string ToString()
    {
        return "{id=" + id.ToString() + ", name=" + name + ", type=" + type.ToString() + ", mass=" + mass.ToString() + "}";
    }
}
