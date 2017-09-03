# VisualStudio Filterable Output Window Extensiosn
Download here https://marketplace.visualstudio.com/items?itemName=nertilpoci.FilterDebugWindow

Provide new output windows which can be filtered by tags so you can debug on section of code at a time

Working on a large project with many developers means that everybody does this **Debug.Write(message)**  the the output window is not very useful since you cannot read your messages with all the messages coming in from different sections of the app. This extension provides a new window which reads the output window and then you can filter the output based on tags

    Debug.WriteLine("@mytag" + message)

   

 You can create an extension that would do this for your tags automatically for you. After this you can select which tags you want to display or not display
