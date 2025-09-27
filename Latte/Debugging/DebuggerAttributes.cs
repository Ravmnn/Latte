using System;

using Latte.UI.Elements.Attributes;


namespace Latte.Debugging;




[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowBoundsDimensionsAndPositionAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowPriorityAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreShowFocusAttribute : ElementAttribute;


[AttributeUsage(AttributeTargets.Class)]
public class DebuggerIgnoreInspection : ElementAttribute;
