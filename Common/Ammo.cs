if(isPackage("City_AmmoSystem"))
	deactivatePackage("City_AmmoSystem");

package City_AmmoSystem
{
	function WeaponImage::onFire(%this, %obj, %slot)
	{
		return Parent::onFire(%this, %obj, %slot);
	}
};
activatePackage("City_AmmoSystem");