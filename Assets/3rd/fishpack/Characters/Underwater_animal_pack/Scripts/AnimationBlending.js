var Anims:AnimationClip[];
var fadeTime:float;

function Start()
{
GetComponent.<Animation>().Play("swim");
}

function Update()
{ 
if(Input.GetButtonDown("Fire1"))
	{
	GetComponent.<Animation>().CrossFade("eat",1);
	}
 if(Input.GetButtonUp("Fire1"))
	{
	GetComponent.<Animation>().CrossFade("swim",1);
	//Switch2toStop();
	//Switch1toPlay();
	}
}


function Switch1toStop () 
{
GetComponent.<Animation>().Blend(Anims[1].name,0,fadeTime);
}

function Switch2toStop ()
{
GetComponent.<Animation>().Blend(Anims[2].name,0,fadeTime);
}

function Switch3toStop ()
{
GetComponent.<Animation>().Blend(Anims[3].name,0,fadeTime);
}

function Switch1toPlay ()
{
GetComponent.<Animation>().Blend(Anims[1].name,1,fadeTime);
}

function Switch2toPlay ()
{
GetComponent.<Animation>().Blend(Anims[2].name,1,fadeTime);
}

function Switch3toPlay ()
{
GetComponent.<Animation>().Blend(Anims[3].name,1,fadeTime);
}


