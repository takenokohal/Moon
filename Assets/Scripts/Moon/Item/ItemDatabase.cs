using System;
using System.Linq;
using Moon.Bullet;
using Moon.Shooter;
using UnityEngine;

namespace Moon.Item
{
    [CreateAssetMenu(menuName = "Create ItemProvider", fileName = "ItemProvider", order = 0)]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private ItemData[] itemDataArray;
        public ItemData[] ItemDataArray => itemDataArray;


        [Serializable]
        public struct ItemData
        {
            [SerializeField] private ShooterType shooterType;
            public ShooterType ShooterType => shooterType;

            [SerializeField] private Sprite sprite;
            public Sprite Sprite => sprite;
            [SerializeField]private Color color;
            public Color Color => color;

            [SerializeField] private string name;
            public string Name => name;
        }
    }
}