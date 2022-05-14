using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    private bool inRange = false;
    private bool isTriggered = false;
    private Consumables consumables;
    private Dictionary<int, float> chancesForDropAmount = new Dictionary<int, float>
    {
        { 1, 40 },
        { 2, 25 },
        { 3, 20 },
        { 4, 10 },
        { 5, 5 },
    };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            inRange = false;
    }

    void Start()
    {
        consumables = Player.instanse.GetComponent<Consumables>();
    }

    private void Update()
    {
        if (!isTriggered && inRange && Input.GetKeyDown(KeyCode.E))
        {
            var consumabelsList = consumables.GetType().GetProperties(
                System.Reflection.BindingFlags.Public|
                System.Reflection.BindingFlags.Instance|
                System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var propertyInfo in consumabelsList) {
                var chance = UnityEngine.Random.value * 100;
                var consumableObj = propertyInfo.GetValue(consumables);
                //Debug.Log("\n[+] Name: " + propertyInfo.Name + "\n[+] Value: " + consumableObj);
                //проверка на тип свойства
                if (!consumableObj.GetType().Equals(1.GetType())) {
                    Debug.Log("Property type is not int!");
                    continue;
                }
                //почему то не хочет кастовать с одной переменной, поэтому создал другую
                var consumableCount = (int) consumableObj;
                Debug.Log("\nCurrent generated number " + chance);
                for (int i = 1; i <= 5; i++) 
                {
                    if (chance <= chancesForDropAmount[i]) 
                    {
                        propertyInfo.SetValue(consumables, consumableCount + i);
                        Debug.Log("\nAdded " + i + " to " + propertyInfo.Name);
                        break;
                    }
                    chance -= chancesForDropAmount[i];
                }
                //propertyInfo.SetValue(consumables, consumableCount + 99);

            }
            //Debug.Log("Created list of consumabels with lenght: " + consumabelsList.Length);
            // Для обучающего уровня
            isTriggered = true;
            ServiceInfo.CheckpointConditionDone = true;

        }
    }
}
