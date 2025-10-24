using UnityEngine;
using System.Collections;
namespace RGame
{
	public interface IListener
	{
		void IPlay();
		void ISuccess();
		void IPause();
		void IUnPause();
		void IGameOver();
		void IOnRespawn();
		void IOnStopMovingOn();
		void IOnStopMovingOff();
	}
}