﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI.Windows.Animations;

namespace UnityEngine.UI.Windows {

	public class WindowComponentBase : WindowObjectElement, IWindowAnimation {
		
		[Header("Animation Info")]
		[SceneEditOnly]
		new public WindowAnimationBase animation;
		
		public ChildsBehaviourMode childsShowMode = ChildsBehaviourMode.Simultaneously;
		public ChildsBehaviourMode childsHideMode = ChildsBehaviourMode.Simultaneously;

		[HideInInspector]
		public List<TransitionInputParameters> animationInputParams = new List<TransitionInputParameters>();
		//[HideInInspector]
		//public CanvasGroup canvas;
		
		[SerializeField][HideInInspector]
		private bool manualShowHideControl = false;

		//private string animationTag = null;

		public override void OnInit() {
			
			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.Init);

			base.OnInit();

		}

		public override void OnDeinit(System.Action callback) {
			
			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.Deinit);

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnDeinit);

			WindowSystem.GetEvents().Clear(this);
			this.animation = null;
			WindowSystem.GetHistoryTracker().Clear(this);
			this.animationInputParams.Clear();

			base.OnDeinit(callback);

		}

		public override void OnWindowOpen() {

			base.OnWindowOpen();

			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.WindowOpen);

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnWindowOpen);

			this.manualShowHideControl = false;

		}

		public override void OnWindowClose() {

			base.OnWindowClose();

			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.WindowClose);

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnWindowClose);

			this.manualShowHideControl = false;

		}

		public override void OnWindowActive() {

			base.OnWindowActive();

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnWindowActive);

			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.WindowActive);

		}

		public override void OnWindowInactive() {

			base.OnWindowInactive();

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnWindowInactive);

			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.WindowInactive);

		}

		public ME.Tweener.MultiTag GetTag() {
			
			//if (this.animationTag == null) this.animationTag = this.GetCustomTag("WindowComponentBase");
			//return this.animationTag;

			return this.GetCustomTag(null);

		}

		public ME.Tweener.MultiTag GetCustomTag(Object custom) {

			return new ME.Tweener.MultiTag() { tag1 = this, tag2 = custom };

			//return string.Format("{0}_{1}", this.GetInstanceID(), custom);

		}

		/*public string GetCustomTag(int custom) {
			
			return string.Format("{0}_{1}", this.GetInstanceID(), custom);

		}*/

		#region Manual Events
		public void HideExcludeChilds(System.Action callback = null, bool immediately = false) {
			
			this.Hide(AppearanceParameters.Default()
			          .ReplaceCallback(callback: callback)
			          .ReplaceImmediately(immediately: immediately)
			          .ReplaceIncludeChilds(includeChilds: false));

		}

		public void Hide(bool immediately) {
			
			this.Hide(AppearanceParameters.Default()
			          .ReplaceImmediately(immediately: immediately));
			
		}
		
		public void Hide(System.Action callback, bool immediately) {
			
			this.Hide(AppearanceParameters.Default()
			          .ReplaceCallback(callback: callback)
			          .ReplaceImmediately(immediately: immediately));
			
		}
		
		public void ShowExcludeChilds(System.Action callback = null, bool resetAnimation = true) {
			
			this.Show(AppearanceParameters.Default()
			          .ReplaceCallback(callback: callback)
			          .ReplaceResetAnimation(resetAnimation: resetAnimation)
			          .ReplaceIncludeChilds(includeChilds: false));
			
		}

		public void Show(bool resetAnimation) {
			
			this.Show(AppearanceParameters.Default()
			          .ReplaceResetAnimation(resetAnimation: resetAnimation));
			
		}

		public void Show(System.Action callback, bool resetAnimation) {

			this.Show(AppearanceParameters.Default()
			          .ReplaceCallback(callback: callback)
			          .ReplaceResetAnimation(resetAnimation: resetAnimation));

		}

		public void ShowHide(bool state) {

			this.ShowHide(state, AppearanceParameters.Default());

		}

		public void ShowHide(bool state, AppearanceParameters parameters) {
			
			if (state == true) {
				
				this.Show(parameters);
				
			} else if (state == false) {
				
				this.Hide(parameters);
				
			}

		}

		public void Show() {

			this.Show(AppearanceParameters.Default());

		}

		public void Hide() {

			this.Hide(AppearanceParameters.Default());

		}
		
		public void Show(AppearanceParameters parameters) {

			if ((this.GetComponentState() == WindowObjectState.Shown ||
				this.GetComponentState() == WindowObjectState.Showing) &&
			    parameters.GetForced(defaultValue: false) == false) {

				parameters.Call();
				return;

			}

			WindowSystem.GetHistoryTracker().Add(this, parameters, HistoryTrackerEventType.ShowManual);

			this.manualShowHideControl = true;

			//parameters = parameters.ReplaceManual(manual: true);

			var callback = parameters.GetCallback(null);
			var parametersResult = parameters.ReplaceCallback(() => {
				
				if (this.GetComponentState() != WindowObjectState.Showing) {
					
					if (callback != null) callback.Invoke();
					return;
					
				}

				this.DoShowEnd_INTERNAL(parameters);
				if (callback != null) callback.Invoke();
				
			});
			
			this.DoShowBegin_INTERNAL(parametersResult);
			
		}

		public void Hide(AppearanceParameters parameters) {
			
			if ((this.GetComponentState() == WindowObjectState.Hidden ||
			    this.GetComponentState() == WindowObjectState.Hiding) &&
			    parameters.GetForced(defaultValue: false) == false) {

				parameters.Call();
				return;
				
			}

			WindowSystem.GetHistoryTracker().Add(this, parameters, HistoryTrackerEventType.HideManual);

			this.manualShowHideControl = true;

			//parameters = parameters.ReplaceManual(manual: true);

			var callback = parameters.GetCallback(null);
			var parametersResult = parameters.ReplaceCallback(() => {
				
				if (this.GetComponentState() != WindowObjectState.Hiding) {
					
					if (callback != null) callback.Invoke();
					return;

				}
				
				this.DoHideEnd_INTERNAL(parameters);
				if (callback != null) callback.Invoke();

			});

			this.DoHideBegin_INTERNAL(parametersResult);

		}
		#endregion

		#region Base Events
		public override void DoShowEnd(AppearanceParameters parameters) {
			
			if (this.manualShowHideControl == true || this.showOnStart == false) {

				return;
				
			}
			
			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.ShowEnd);

			this.DoShowEnd_INTERNAL(parameters);

		}

		public override void DoShowBegin(AppearanceParameters parameters) {

			if (parameters.GetManual(false) == true) {

				this.Show(parameters);
				return;

			}

			if (this.manualShowHideControl == true || this.showOnStart == false) {

				parameters.Call();
				return;

			}
			
			WindowSystem.GetHistoryTracker().Add(this, parameters, HistoryTrackerEventType.ShowBegin);

			this.DoShowBegin_INTERNAL(parameters);

		}

		public override void DoHideEnd(AppearanceParameters parameters) {
			
			if (this.manualShowHideControl == true) {
				
				return;
				
			}
			
			WindowSystem.GetHistoryTracker().Add(this, HistoryTrackerEventType.HideEnd);

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnHideEnd);

			this.DoHideEnd_INTERNAL(parameters);
			
		}

		public override void DoHideBegin(AppearanceParameters parameters) {

			if (parameters.GetManual(false) == true) {

				this.Hide(parameters);
				return;

			}

			if (this.manualShowHideControl == true) {
				
				parameters.Call();
				return;
				
			}
			
			WindowSystem.GetHistoryTracker().Add(this, parameters, HistoryTrackerEventType.HideBegin);

			this.DoHideBegin_INTERNAL(parameters);

		}
		#endregion

		private void DoShowBegin_INTERNAL(AppearanceParameters parameters) {

			if ((this.GetComponentState() == WindowObjectState.Showing ||
			    this.GetComponentState() == WindowObjectState.Shown) &&
			    parameters.GetForced(defaultValue: false) == false) {
				
				parameters.Call();
				return;
				
			}

			this.SetComponentState(WindowObjectState.Showing);

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnShowBegin);

			var parametersCallback = parameters.callback;
			System.Action onResult = () => {

				if (parametersCallback != null) parametersCallback.Invoke();

			};

			#if DEBUGBUILD
			Profiler.BeginSample("WindowComponentBase::OnShowBegin()");
			#endif

			WindowSystem.RunSafe(this.OnShowBegin);
			WindowSystem.RunSafe(this.OnShowBegin, parameters);

			#if DEBUGBUILD
			Profiler.EndSample();
			#endif

			var includeChilds = parameters.GetIncludeChilds(defaultValue: true);
			var childsBehaviour = parameters.GetChildsBehaviourMode(this.childsShowMode);

			if (childsBehaviour == ChildsBehaviourMode.Consequentially) {

				var resetAnimation = parameters.GetResetAnimation(defaultValue: true);
				if (resetAnimation == true) {

					if (includeChilds == true) {

						this.DoResetState();

					} else {
						
						this.SetResetState();

					}

				}

			}

			#region Include Childs
			if (includeChilds == false) {

				// without childs
				this.DoShowBeginAnimation_INTERNAL(onResult, parameters);
				return;

			}
			#endregion

			if (childsBehaviour == ChildsBehaviourMode.Simultaneously) {

				#region Childs Simultaneously
				var counter = 0;
				System.Action callback = () => {

					++counter;
					if (counter < 2) return;

					onResult.Invoke();
					
				};

				this.DoShowBeginAnimation_INTERNAL(callback, parameters);

				ME.Utilities.CallInSequence(callback, this.subComponents, (e, c) => {

					e.DoShowBegin(parameters.ReplaceCallback(c));

				});
				#endregion

			} else if (childsBehaviour == ChildsBehaviourMode.Consequentially) {

				#region Childs Consequentially
				ME.Utilities.CallInSequence(() => {

					this.DoShowBeginAnimation_INTERNAL(onResult, parameters);
					
				}, this.subComponents, (e, c) => {

					e.DoShowBegin(parameters.ReplaceCallback(c));
					
				}, waitPrevious: true);
				#endregion

			}

		}
		
		private void DoShowEnd_INTERNAL(AppearanceParameters parameters) {

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnShowEnd);

			base.DoShowEnd(parameters);
			
		}
		
		private void DoHideBegin_INTERNAL(AppearanceParameters parameters) {
			
			if ((this.GetComponentState() == WindowObjectState.Hiding ||
			    this.GetComponentState() == WindowObjectState.Hidden) &&
			    parameters.GetForced(defaultValue: false) == false) {
				
				parameters.Call();
				return;
				
			}

			this.SetComponentState(WindowObjectState.Hiding);

			WindowSystem.GetEvents().Raise(this, WindowEventType.OnHideBegin);

			var parametersCallback = parameters.callback;
			System.Action onResult = () => {

				if (parametersCallback != null) parametersCallback.Invoke();

			};

			#if DEBUGBUILD
			Profiler.BeginSample("WindowComponentBase::OnHideBegin()");
			#endif

			WindowSystem.RunSafe(this.OnHideBegin);
			WindowSystem.RunSafe(this.OnHideBegin, parameters);

			#if DEBUGBUILD
			Profiler.EndSample();
			#endif

			var includeChilds = parameters.GetIncludeChilds(defaultValue: true);
			#region Include Childs
			if (includeChilds == false) {

				// without childs
				this.DoHideBeginAnimation_INTERNAL(onResult, parameters);
				return;
				
			}
			#endregion
			
			var childsBehaviour = parameters.GetChildsBehaviourMode(this.childsHideMode);
			if (childsBehaviour == ChildsBehaviourMode.Simultaneously) {
				
				#region Childs Simultaneously
				var counter = 0;
				System.Action callback = () => {
					
					++counter;
					if (counter < 2) return;
					
					onResult.Invoke();
					
				};
				
				this.DoHideBeginAnimation_INTERNAL(callback, parameters);
				
				ME.Utilities.CallInSequence(callback, this.subComponents, (e, c) => {
					
					e.DoHideBegin(parameters.ReplaceCallback(c));
					
				});
				#endregion
				
			} else if (childsBehaviour == ChildsBehaviourMode.Consequentially) {
				
				#region Childs Consequentially
				ME.Utilities.CallInSequence(() => {
					
					this.DoHideBeginAnimation_INTERNAL(onResult, parameters);
					
				}, this.subComponents, (e, c) => {
					
					e.DoHideBegin(parameters.ReplaceCallback(c));
					
				}, waitPrevious: true);
				#endregion
				
			}

		}
		
		private void DoHideEnd_INTERNAL(AppearanceParameters parameters) {

			base.DoHideEnd(parameters);
			
		}
		
		private void DoShowBeginAnimation_INTERNAL(System.Action callback, AppearanceParameters parameters) {
			
			var resetAnimation = parameters.GetResetAnimation(defaultValue: true);
			var immediately = parameters.GetImmediately(defaultValue: false);
			var delay = parameters.GetDelay(defaultValue: 0f);
			
			System.Action callbackInner = () => {
				
				if (this.animation != null) {
					
					if (resetAnimation == true) this.SetResetState();
					
					if (immediately == true) {
						
						this.animation.SetInState(this.animationInputParams, this.GetWindow(), this);
						callback.Invoke();
						
					} else {
						
						this.animation.Play(this.GetWindow(), this.animationInputParams, this, true, callback);
						
					}
					
				} else {
					
					callback.Invoke();
					
				}

			};

			if (TweenerGlobal.instance != null) {

				var tag = this.BreakState();

				if (immediately == false && delay > 0f) {

					TweenerGlobal.instance.addTween(this, delay, 0f, 0f).tag(tag).onComplete(() => {

						callbackInner.Invoke();

					}).onCancel((obj) => {

						callbackInner.Invoke();

					});

				} else {

					callbackInner.Invoke();

				}

			} else {
				
				callbackInner.Invoke();
				
			}

		}
		
		private void DoHideBeginAnimation_INTERNAL(System.Action callback, AppearanceParameters parameters) {

			var resetAnimation = parameters.GetResetAnimation(defaultValue: false);
			var immediately = parameters.GetImmediately(defaultValue: false);
			var delay = parameters.GetDelay(defaultValue: 0f);

			System.Action callbackInner = () => {
				
				if (this.animation != null) {
					
					if (resetAnimation == true) this.SetResetState();
					
					if (immediately == true) {
						
						this.animation.SetOutState(this.animationInputParams, this.GetWindow(), this);
						callback.Invoke();
						
					} else {
						
						this.animation.Play(this.GetWindow(), this.animationInputParams, this, false, callback);
						
					}
					
				} else {
					
					callback.Invoke();
					
				}

			};

			if (TweenerGlobal.instance != null) {

				var tag = this.BreakState();

				if (immediately == false && delay > 0f) {

					TweenerGlobal.instance.addTween(this, delay, 0f, 0f).tag(tag).onComplete(() => {

						callbackInner.Invoke();

					}).onCancel((obj) => {
						
						callbackInner.Invoke();

					});

				} else {

					callbackInner.Invoke();

				}

			} else {

				callbackInner.Invoke();

			}

		}

		public void DoBreakState() {

			this.BreakState();

			for (int i = 0; i < this.subComponents.Count; ++i) (this.subComponents[i] as WindowComponentBase).DoBreakState();

		}

		public void DoResetState() {

			this.SetResetState();

			for (int i = 0; i < this.subComponents.Count; ++i) (this.subComponents[i] as WindowComponentBase).DoResetState();

		}

		public ME.Tweener.MultiTag BreakState() {

			var tag = (this.animation != null ? this.GetCustomTag(this.animation) : this.GetTag());
			TweenerGlobal.instance.removeTweens(tag);

			return tag;

		}

		/// <summary>
		/// Set up `reset` state to animation.
		/// </summary>
		public void SetResetState() {
			
			if (this.animation != null) this.animation.SetResetState(this.animationInputParams, this.GetWindow(), this);
			
		}
		
		/// <summary>
		/// Set up `in` state to animation.
		/// </summary>
		public void SetInState() {
			
			if (this.animation != null) this.animation.SetInState(this.animationInputParams, this.GetWindow(), this);
			
		}
		
		/// <summary>
		/// Set up `out` state to animation.
		/// </summary>
		public void SetOutState() {
			
			if (this.animation != null) this.animation.SetOutState(this.animationInputParams, this.GetWindow(), this);
			
		}
		
		/// <summary>
		/// Gets the duration of the animation.
		/// </summary>
		/// <returns>The animation duration.</returns>
		/// <param name="forward">If set to <c>true</c> forward.</param>
		public virtual float GetAnimationDuration(bool forward) {
			
			if (this.animation != null) {
				
				return this.animation.GetDuration(this.animationInputParams, forward);
				
			}
			
			return 0f;
			
		}

		#if UNITY_EDITOR
		public override void OnValidateEditor() {

			base.OnValidateEditor();

			this.Update_EDITOR();

		}

		[HideInInspector]
		private List<TransitionInputParameters> componentsToDestroy = new List<TransitionInputParameters>();
		[HideInInspector][SerializeField]
		private WindowAnimationBase lastAnimation;

		private void Update_EDITOR() {

			this.animationInputParams = this.animationInputParams.Where((i) => i != null).ToList();
			
			//this.canvas = this.GetComponent<CanvasGroup>();

			if (ME.EditorUtilities.IsPrefab(this.gameObject) == true) return;

			var changed = (this.lastAnimation != this.animation);
			if (changed == true) {
				
				this.componentsToDestroy.Clear();

				if (this.animation == null || this.lastAnimation != this.animation) {
					
					var ps = this.GetComponents<TransitionInputParameters>();
					foreach (var p in ps) {
						
						this.componentsToDestroy.Add(p);
						
					}

				}
				
				if (this.animation != null) {
					
					this.animation.ApplyInputParameters(this);
					
				}

				this.lastAnimation = this.animation;

				this.DelayDestroy_EDITOR();

			}

		}

		private void DelayDestroy_EDITOR() {

			var toDestroy = this.componentsToDestroy.ToArray();

			UnityEditor.EditorApplication.CallbackFunction delayCall = null;
			delayCall = () => {

				foreach (var c in toDestroy) {

					Component.DestroyImmediate(c);

				}

				this.animationInputParams = this.animationInputParams.Where((i) => i != null).ToList();

				UnityEditor.EditorApplication.delayCall -= delayCall;

			};

			UnityEditor.EditorApplication.delayCall += delayCall;

		}
		#endif

	}

}