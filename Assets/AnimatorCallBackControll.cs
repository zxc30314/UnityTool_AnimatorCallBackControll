using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;


public class AnimatorCallBackControll : MonoBehaviour
{
    public Animator ator;
    [ListDrawerSettings(HideAddButton = true, HideRemoveButton = false, ListElementLabelName = "Name")]
    public List<AnimatorStateCallBack> AnimatorStateCallBack = new List<AnimatorStateCallBack>();
    List<AnimatorStateCallBack> tempAnimatorStateCallBack = new List<AnimatorStateCallBack>();
    string Root;
    string path = "";
    string fullPath = "";
    // Use this for initialization

    void Start()
    {

        ator = GetComponentInChildren<Animator>();




    }
#if UNITY_EDITOR
    [Button]
    private void ReadAllStates()
    {
        path = "";
        ator = GetComponentInChildren<Animator>();
        print(ator.runtimeAnimatorController);
        RuntimeAnimatorController myController = ator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;
        if (myOverrideController != null)
        {
            myController = myOverrideController.runtimeAnimatorController;
        }

        UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)myController;
        UnityEditor.Animations.AnimatorControllerLayer[] allLayer = animatorController.layers;

        tempAnimatorStateCallBack.Clear();
        for (int i = 0; i < allLayer.Length; i++)
        {
            Root = allLayer[i].name;
            GetAllEvent(allLayer[i].stateMachine);
        }


        //比對


        for (int l = 0; l < AnimatorStateCallBack.Count; l++)
        {

            for (int m = 0; m < tempAnimatorStateCallBack.Count; m++)
            {

                if (tempAnimatorStateCallBack[m].HashCode == AnimatorStateCallBack[l].HashCode)
                {

                    tempAnimatorStateCallBack[m] = new AnimatorStateCallBack(AnimatorStateCallBack[l]);
                    break;
                }


            }



        }

        AnimatorStateCallBack = new List<AnimatorStateCallBack>();
        AnimatorStateCallBack.AddRange(tempAnimatorStateCallBack);
     
        print(tempAnimatorStateCallBack.Count);
        print(AnimatorStateCallBack.Count);
    }

    //遞迴
    void GetAllEvent(UnityEditor.Animations.AnimatorStateMachine root)
    {


        UnityEditor.Animations.ChildAnimatorStateMachine[] stateMachines = root.stateMachines;

        string tempPath = path;

        foreach (UnityEditor.Animations.ChildAnimatorStateMachine child in stateMachines)

        {

            path += child.stateMachine.name + ".";
            GetAllEvent(child.stateMachine);

        }
        UnityEditor.Animations.ChildAnimatorState[] states = root.states;
        foreach (UnityEditor.Animations.ChildAnimatorState state in states)

        {

            fullPath = Root + "." + tempPath + state.state.name;
            path = "";

            foreach (var behaviour in state.state.behaviours)
            {
                if (behaviour.GetType().Name == "FSM")
                {
                    tempAnimatorStateCallBack.Add(new AnimatorStateCallBack(fullPath, Animator.StringToHash(fullPath)));
               
                }

            }


        }

    }

    void Sort(List<AnimatorStateCallBack> animatorStateCallBack)
    {
        AnimatorStateCallBack temp;
        bool swapped;
        for (int i = 0; i < animatorStateCallBack.Count; i++)
        {
            swapped = false;
            for (int j = 0; j < animatorStateCallBack.Count - 1 - i; j++)
                if (animatorStateCallBack[j].HashCode > animatorStateCallBack[j + 1].HashCode)
                {
                    temp = animatorStateCallBack[j];
                    animatorStateCallBack[j] = animatorStateCallBack[j + 1];
                    animatorStateCallBack[j + 1] = temp;
                    if (!swapped)
                        swapped = true;
                }
            if (!swapped)
                return;
        }
    }
#endif
    // Update is called once per frame
    void Update()
    {


    }
    public override void AnimationOnPlay(AnimatorStateInfo info)
    {

        for (int i = 0; i < AnimatorStateCallBack.Count; i++)
        {
            if (AnimatorStateCallBack[i].HashCode == info.fullPathHash)
            {
                AnimatorStateCallBack[i].OnPlay.Invoke();

            }
        }
    }
    public override void AnimationOnPlayEnd(AnimatorStateInfo info)
    {


        for (int i = 0; i < AnimatorStateCallBack.Count; i++)
        {

            if (AnimatorStateCallBack[i].HashCode == info.fullPathHash)
            {


                AnimatorStateCallBack[i].OnPlayEnd.Invoke();
            }
        }


    }




}
[System.Serializable]
public class AnimatorStateCallBack
{

    [ReadOnly]
    public string Name, Path;
    [HideInInspector]
    public int HashCode;

    public UnityEvent OnPlay, OnPlayEnd;
    public AnimatorStateCallBack(AnimatorStateCallBack animatorStateCallBack)
    {
        Name = animatorStateCallBack.Path.Split('.')[animatorStateCallBack.Path.Split('.').Length - 1];
        Path = animatorStateCallBack.Path;
        HashCode = animatorStateCallBack.HashCode;
        OnPlay = animatorStateCallBack.OnPlay;
        OnPlayEnd = animatorStateCallBack.OnPlayEnd;
    }

    public AnimatorStateCallBack(string path, int hashCode)
    {
        Name = path.Split('.')[path.Split('.').Length - 1];
        Path = path;
        HashCode = hashCode;


    }




}
