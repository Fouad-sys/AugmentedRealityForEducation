# AugmentedRealityForEducation

This project was conducted as part of a semester project in the EPFL CHILI lab on using Augmented Reality for Education for Teachers teaching an Online Classroom.

The project can be run on the Microsoft HoloLens 1 after deployment through Visual Studio or locally br running the Main 1 Unity Scene.

It requires downloading and installing The Mixed Reality Feature Tool (https://www.microsoft.com/en-us/download/details.aspx?id=102778) and importing the Mixed Reality ToolKit Foundation and the Mixed Reality OpenXR Plugin to the Unity project.

The necessary Build settings to build the project in Unity are as seen below:

![Screenshot (18)](https://github.com/Fouad-sys/AugmentedRealityForEducation/assets/61212919/7a82dd41-4e26-4bee-b8c2-5305e29d5bc9)

The necessary ProjectSettings to change in the player tab are as follows:

![Screenshot (27)](https://github.com/Fouad-sys/AugmentedRealityForEducation/assets/61212919/4a182502-dd1d-4950-a4fb-aa3e34e427f6)

Upon build, the user should open the built Visual Studio .sln file containing the same name as that of the Unity project. Then, select the release mode, x86 and Remote Machine in the Top Menu panel. Afterwards, select Project->Properties and in the Debugging panel, next to Machine Name, enter the IP address of your HoloLens as shown below.

![Screenshot (35)](https://github.com/Fouad-sys/AugmentedRealityForEducation/assets/61212919/484712f3-3602-48ff-98b3-d0bba2cfbc3e)

Latsly, deploy the project onto your HoloLens by selecting Build->Deploy Solution.


