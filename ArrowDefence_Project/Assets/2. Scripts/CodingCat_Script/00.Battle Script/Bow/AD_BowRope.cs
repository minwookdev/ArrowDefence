namespace ActionCat
{
	using System.Collections;
    using UnityEngine;

    public class AD_BowRope : MonoBehaviour
    {
		/// <summary>
		/// The rope1 and rope2 line renderer referecnes.
		/// </summary>
		public LineRenderer rope1, rope2;

		/// <summary>
		/// The bow top point reference(used to as the first point of the top rope/rope1).
		/// </summary>
		public Transform bowLeftPoint;

		/// <summary>
		/// The bow bottom point reference (used to as the first point of the bottom rope/rope2).
		/// </summary>
		public Transform bowRightPoint;

		/// <summary>
		/// The arrow catch point (a point in which the arrow will be held).
		/// </summary>
		public Transform arrowCatchPoint;

		/// <summary>
		/// The color of the rope. [now white]
		/// </summary>
		public Color color = new Color(1f, 1f, 1f, 1f);

		/// <summary>
		/// The width of the rope.
		/// </summary>
		public Vector2 width = new Vector2(0.03f, 0.03f);

		/// <summary>
		/// The rope material.
		/// </summary>
		public Material ropeMaterial;

		/// <summary>
		/// The static instance for this class.
		/// </summary>
		public static AD_BowRope instance;

		private void Awake()
		{
			//static Instance Settings
			if (instance == null)
			{
				instance = this;
			}
		}

		// Use this for initialization
		void Start()
		{
			//Setting up ropes width
			rope1.startWidth = width.x;
			rope1.endWidth = width.y;

			rope2.startWidth = width.x;
			rope2.endWidth = width.y;

			//Setting up material color
			ropeMaterial.color = color;

			//Setting up ropes material
			rope1.material = ropeMaterial;
			rope2.material = ropeMaterial;

			StartCoroutine(DrawRope());
		}

		//void Update() {
		//	DrawRope();
		//}

		void RopeUpdate() {
			//Draw the rope(top,bottom) of the bow
			if (arrowCatchPoint == null) {
				rope1.SetPosition(0, bowLeftPoint.position);
				rope1.SetPosition(1, bowRightPoint.position);

				rope2.SetPosition(0, Vector3.zero);
				rope2.SetPosition(1, Vector3.zero);

				return;
			}

			rope1.SetPosition(0, bowLeftPoint.position);
			rope1.SetPosition(1, arrowCatchPoint.position);

			rope2.SetPosition(0, arrowCatchPoint.position);
			rope2.SetPosition(1, bowRightPoint.position);
		}

		IEnumerator DrawRope(){
			while(GameManager.Instance.GameState != GAMESTATE.STATE_ENDBATTLE) {
				yield return null;
				//Draw the rope(top,bottom) of the bow
				if (arrowCatchPoint == null) { //Before Pulling The Bow.
					rope1.SetPosition(0, bowLeftPoint.position);
					rope1.SetPosition(1, bowRightPoint.position);

					rope2.SetPosition(0, Vector3.zero);
					rope2.SetPosition(1, Vector3.zero);
				}
				else {						   //Pulling Start.
					rope1.SetPosition(0, bowLeftPoint.position);
					rope1.SetPosition(1, arrowCatchPoint.position);

					rope2.SetPosition(0, arrowCatchPoint.position);
					rope2.SetPosition(1, bowRightPoint.position);
				}
			}

			//Battle End. Set Last Rope Position.
			rope1.SetPosition(0, bowLeftPoint.position);
			rope1.SetPosition(1, bowRightPoint.position);

			rope2.SetPosition(0, Vector3.zero);
			rope2.SetPosition(1, Vector3.zero);
		}
	}
}
