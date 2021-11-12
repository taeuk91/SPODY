using UnityEngine;

public class ShowNextImage : MonoBehaviour
{
    GameObject[] objects;

    private void Awake()
    {
        objects = new GameObject[this.transform.childCount];

        for(int i=0; i<this.transform.childCount; i++)
        {
            objects[i] = this.transform.GetChild(i).gameObject;
        }
    }

    int index = 0;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Show(index);

            index++;
            if(index >= this.transform.childCount)
                index = 0;
        }
    }

    void Show(int index)
    {
        for(int i=0; i<this.transform.childCount; i++)
        {
            objects[i].SetActive(i ==index);
        }
    }
}
