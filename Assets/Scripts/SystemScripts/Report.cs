using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Report {
    public List<String> reportTypes;
    public List<String> itemNames;
    public List<int> itemEnergyCosts;
    public float timeMade;
    public List<float> itemActiveTime;
    public bool correct;
    public String room;
    public int totalEnergyCost;

    public Report(List<String> reportTypes, List<String> itemNames, List<int> itemEnergyCosts, float timeMade, List<float> itemActiveTime, bool correct, String room, int totalEnergyCost) {
        this.reportTypes = reportTypes;
        this.itemNames = itemNames;
        this.itemEnergyCosts = itemEnergyCosts;
        this.timeMade = timeMade;
        this.itemActiveTime = itemActiveTime;
        this.correct = correct;
        this.room = room;
        this.totalEnergyCost = totalEnergyCost;
    }

    public static String ToString(Report report) {
        String reportString = "Report: \n";
        reportString += "Report Types: ";
        foreach(String reportType in report.reportTypes) {
            reportString += reportType + ", ";
        }
        reportString += "\nItem Names: ";
        foreach(String itemName in report.itemNames) {
            reportString += itemName + ", ";
        }
        reportString += "\nItem Energy Costs: ";
        foreach(int itemEnergyCost in report.itemEnergyCosts) {
            reportString += itemEnergyCost + ", ";
        }
        reportString += "\nTime Made: " + report.timeMade;
        reportString += "\nItem Active Time: ";
        foreach(float itemActiveTime in report.itemActiveTime) {
            reportString += itemActiveTime + ", ";
        }
        reportString += "\nCorrect: " + report.correct;
        reportString += "\nRoom: " + report.room;
        reportString += "\nTotal Energy Cost: " + report.totalEnergyCost;
        return reportString;
    }
}
