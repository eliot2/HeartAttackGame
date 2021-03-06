﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackpackSystem : MonoBehaviour {


    public Animator[] animators = new Animator[4];
    public Transform player;
    public GameObject properLookItem;
    private Ray ray;
    public Camera p1Cam;
    Rigidbody body;
    Rigidbody equipBody;

    public Image[] uiImages = new Image[4];
    public GameObject[] backpackSlots = new GameObject[4];
    bool hasItemGrab = false;

    public float forwardVal = 0.0f;
    public float rightVal = 0.0f;
    public float upVal = 0.0f;
    Transform equippedItem;

    void Start() {
        player = GetComponent<Transform>();
        Player1Input.dpadPressed += UpdateAnimators;
        Player1Input.dpadPressed += CheckShowItem;
        Player1Input.aButtonAndDpad += AddLookItem;
        Player1Input.aButtonPressed += AddLookItemDrag;
        Player1Events.lookItem += StoreLookItem;
        Player1Events.lookNothing += ClearLookItem;
    }

    void Update() {
        if (properLookItem != null) { Debug.Log(properLookItem.name); }
        if (hasItemGrab) {
            ray = p1Cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (body != null) { body.transform.position = ray.GetPoint(2); }
        }
    }


    void UpdateAnimators(Player1Input.DpadInputs input) {
        for (int i = 0; i < 4; i++) {
            animators[i].SetBool("UIDpadAnim", i == (int)input);
        }
    }

    void CheckShowItem(Player1Input.DpadInputs input) {
        if (backpackSlots[(int)input] != null) {
            for (int i = 0; i < 4; i++) {
                if (i == (int)input) {
                    //backpackSlots[(int)input].transform.position = p1Cam.transform.position + (p1Cam.transform.forward * forwardVal) + (p1Cam.transform.right * rightVal) + (p1Cam.transform.up * upVal) ;
                    backpackSlots[(int)input].transform.SetParent(p1Cam.transform);
                    backpackSlots[(int)input].transform.localEulerAngles = new Vector3(85.43121f, 352.1493f, 352.3017f);
                    backpackSlots[(int)input].transform.localPosition = new Vector3(0.2835201f, -0.2642593f, 0.2328405f);
                    backpackSlots[(int)input].GetComponent<Collider>().enabled = false;
                    backpackSlots[(int)input].SetActive(true);
                    backpackSlots[(int)input].GetComponent<Rigidbody>().isKinematic = true;
                    equippedItem = backpackSlots[(int)input].transform;
                } else {
                    if (backpackSlots[i] != null) {
                        backpackSlots[i].SetActive(false);
                    }
                }
            }
        }
    }

    void StoreLookItem(GameObject item) {
        properLookItem = item;
    }

    void AddLookItemDrag(Player1Input.DpadInputs input) {
        if (properLookItem != null) {
            if (hasItemGrab) {
                Debug.Log("Has item");
                Debug.Log(properLookItem.name);
                properLookItem.transform.SetParent(null);
                body.useGravity = true;
                properLookItem = null;
                hasItemGrab = false;
            } else if (properLookItem.CompareTag("ItemGrab")) {
                if (properLookItem != null) {
                    properLookItem.transform.SetParent(player.transform);
                    body = properLookItem.GetComponent<Rigidbody>();
                    body.useGravity = false;
                    body.transform.position = ray.GetPoint(2);
                    hasItemGrab = true;
                }
            }
        }
    }

    void AddLookItem(Player1Input.DpadInputs input) {
        ray = p1Cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        if (properLookItem != null) {
            if (properLookItem.CompareTag("ItemPickup")) {//Add item to backpack
                properLookItem.transform.SetParent(player);
                body = properLookItem.GetComponent<Rigidbody>();
                body.useGravity = false;
                body.transform.gameObject.SetActive(false);
                if (backpackSlots[(int)input] != null) {
                    backpackSlots[(int)input].SetActive(true);
                    backpackSlots[(int)input].transform.SetParent(null);
                    equipBody = backpackSlots[(int)input].GetComponent<Rigidbody>();
                    equipBody.isKinematic = false;
                    equipBody.useGravity = true;
                    equipBody.gameObject.GetComponent<Collider>().enabled = true;
                    backpackSlots[(int)input] = properLookItem;
                }
                //Debug.Log(properLookItem.name);
                backpackSlots[(int)input] = properLookItem;
                uiImages[(int)input].sprite = properLookItem.GetComponent<Item>().spriteImage;
                properLookItem = null;
            }
        }

    }

    private void ClearLookItem(GameObject item) {
        properLookItem = null;
    }
}
