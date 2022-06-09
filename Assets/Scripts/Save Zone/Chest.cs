using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private Sprite openChestSprite;

    private SpriteRenderer sprite;
    private TriggerZone trigger;
    private Consumables consumables;
    private PressActionKey pressActionKey;

    private Sprite normalSprite;
    //{itemAmount, chance}
    private Dictionary<int, float> chancesForDropAmount = new Dictionary<int, float>
    {
        { 0, 40 },
        { 1, 25 },
        { 2, 20 },
        { 3, 10 },
        { 4, 5 },
    };

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);
            sprite.sprite = normalSprite;
        }
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();

        normalSprite = sprite.sprite;
    }

    private void Start()
    {
        consumables = Player.instanse.GetComponent<Consumables>();
    }

    private void Update()
    {
        if (canBeUsed && trigger.IsTriggered && Input.GetKeyDown(KeyCode.E))
        {
            //���� �������������� �������
            System.Random rng = new System.Random();
            var consumabelsList = consumables.GetType().GetProperties(
                System.Reflection.BindingFlags.Public|
                System.Reflection.BindingFlags.Instance|
                System.Reflection.BindingFlags.DeclaredOnly);
            
            //var anyAmountIncreased = false;
            var amountGenerated = rng.Next(1,5);
            /*Debug.Log("Consumabels in chest generated: "+ amountGenerated);*/
            foreach (var propertyInfo in consumabelsList.OrderBy(x => rng.Next()).ToList())
            {
                if (amountGenerated == 0) break;
                /*Debug.Log("||||||||||||||||||||||||||||||||||||||||||||||||");
                Debug.Log(propertyInfo.Name);
                Debug.Log("[+] Current Remainer: " + amountGenerated);*/
                var consumableObj = propertyInfo.GetValue(consumables);
                if (!consumableObj.GetType().Equals(1.GetType()))
                {
                    Debug.Log("Property type is not int!");
                    continue;
                }
                var currentCount = (int)consumableObj;
                var randomAmount = rng.Next(1,amountGenerated);
                /*Debug.Log("[+] Current count: " + currentCount);
                Debug.Log("[+] Random amount: " + randomAmount);*/
                propertyInfo.SetValue(consumables, currentCount + randomAmount);
                //if (((int)propertyInfo.GetValue(consumables) - currentCount) > 0) anyAmountIncreased = true;
                amountGenerated -= (int)propertyInfo.GetValue(consumables) - currentCount;
            }
            if (amountGenerated>0) {
                /*Debug.Log("unlucky");*/
                var notFullConsumabelsList = consumabelsList
                    .Where(x => (int)x.GetValue(consumables) < Consumables.MaxCount)
                    .ToList();
                var randomConsumable = notFullConsumabelsList[rng.Next(notFullConsumabelsList.Count())];
                randomConsumable.SetValue(consumables, (int)randomConsumable.GetValue(consumables) + amountGenerated);
            }
            /*foreach (var propertyInfo in consumabelsList)
            {
                var chance = UnityEngine.Random.value * 100;
                var consumableObj = propertyInfo.GetValue(consumables);

                //Debug.Log("\n[+] Name: " + propertyInfo.Name + "\n[+] Value: " + consumableObj);

                //�������� �� ��� ��������
                if (!consumableObj.GetType().Equals(1.GetType())) {
                    Debug.Log("Property type is not int!");
                    continue;
                }

                //������ �� �� ����� ��������� � ����� ����������, ������� ������ ������
                var consumableCount = (int) consumableObj;
                *//*Debug.Log("\nCurrent generated number " + chance);*//*
                for (int i = 0; i <= 4; i++) 
                {
                    if (chance <= chancesForDropAmount[i]) 
                    {
                        propertyInfo.SetValue(consumables, consumableCount + i);
                        anyAmountIncreased = true;
                        Debug.Log("\nAdded " + i + " to " + propertyInfo.Name);
                        break;
                    }
                    chance -= chancesForDropAmount[i];
                }

                //���� ����� �� ����� ������ ������ ���������� ���������� ��������� ���-�� (����� ����� �������� �� ��� ������)
                if (!anyAmountIncreased) 
                {
                    Debug.Log("Unlucky random! (For each property 0 amount increased)");
                    consumabelsList[(int)(UnityEngine.Random.value * 100) % 5].SetValue(consumables, consumableCount+1+((int)(UnityEngine.Random.value * 100) % 5));
                }

                //propertyInfo.SetValue(consumables, consumableCount + 99);

            }*/

            //Debug.Log("Created list of consumabels with lenght: " + consumabelsList.Length);

            CanBeUsed = false;
            ServiceInfo.CheckpointConditionDone = true; // ��� ���������� ������
            sprite.sprite = openChestSprite;
        }
    }
}
