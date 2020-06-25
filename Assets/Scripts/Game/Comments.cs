using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comments : MonoBehaviour
{
    private List<string> badComments;
    private List<string> sosoComments;
    private List<string> goodComments;
    private List<string> greatComments;
    private List<string> perfectComments;
    private System.Random random;

    public Comments()
    {
        badComments = new List<string>();
        sosoComments = new List<string>();
        goodComments = new List<string>();
        greatComments = new List<string>();
        perfectComments = new List<string>();
        random = new System.Random();
        CreateBadComments();
        CreateSosoComments();
        CreateGoodComments();
        CreateGreatComments();
        CreatePerfectComments();
    }

    private void CreateBadComments()
    {
        badComments.Add("加油！");
        badComments.Add("有很大的进步空间~");
    }
    private void CreateSosoComments()
    {
        sosoComments.Add("完成的不错，下次一定会更好的~");
    }
    private void CreateGoodComments()
    {
        goodComments.Add("太棒了，继续超越自己呀！");
    }
    private void CreateGreatComments()
    {
        greatComments.Add("太厉害了！");
    }
    private void CreatePerfectComments()
    {
        perfectComments.Add("完美！");
    }

    public string GetBadComments()
    {  
        int index = random.Next(badComments.Count);
        string comment = badComments[index];
        return comment;
    }

    public string GetSosoComments()
    {
        int index = random.Next(sosoComments.Count);
        string comment = sosoComments[index];
        return comment;
    }

    public string GetGoodComments()
    {
        int index = random.Next(goodComments.Count);
        string comment = goodComments[index];
        return comment;
    }

    public string GetGreatComments()
    {
        int index = random.Next(greatComments.Count);
        string comment = greatComments[index];
        return comment;
    }

    public string GetPerfectComments()
    {
        int index = random.Next(perfectComments.Count);
        string comment = perfectComments[index];
        return comment;
    }
}
