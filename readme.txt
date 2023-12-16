Creating dynamic objects:


Generally:
	Add the 'DynamicData' script to each object, then change it's type to whatever you want it to be
	Make sure it has the 'dynamic' tag (top right).
	Make sure each dynamic object is the child of a room that has the 'room' tag
	


Object Disappearance:
	Just change the type in the script to "Object Disappearance"


Object Movement:
	Same as disappearance (can go through walls ATM so try to put in the center of rooms if possible)


Object Change:
	Put the object you want to be the replacement as the child of the original object
	Then, move the replacement object to exactly -10 from the original object on the Y axis
	finally, change the type of the script to "object change"


Light:
	Increases the intensity of the light by 1.2 (will probably change this to be random later)
	Change the type to 'light' in the script


Creature:
	WIP	