﻿using cakeslice;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[HideInInspector]
	public Outline _outline;
	[HideInInspector]
	public BoxCollider _collider;

	[SerializeField]
	private MeshRenderer _frontFaceRenderer;
	[SerializeField]
	private MeshRenderer _backFaceRenderer;


	private CardModel _card;

	private float _maxY;
	private float _minY;

	private void Awake()
	{
		_outline = GetComponent<Outline>();
		_outline.OnHighlightEvent += OnHiglightEvent;

		_minY = GameObject.Find("CardLocators").transform.position.y;
		_maxY = _minY + .1f;

		_collider = GetComponent<BoxCollider>();
	}

	public void SetValues(CardModel card)
	{
		if (card != null)
		{
			_card = card;
			_card.Controller = this;
			_card.GameObject = gameObject;
			_frontFaceRenderer.material.SetTexture("_MainTex", _card.FrontTexture);
			_backFaceRenderer.material.SetTexture("_MainTex", _card.DeckTexture);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!_card.IsTweening)
			_outline.enabled = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!_card.IsTweening)
			_outline.enabled = _card.IsSelected;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!_card.IsTweening)
		{
			var selectedCards = DeckManager.Instance.DealtCards.FindAll(card => card.IsSelected);

			if (selectedCards.Count < 3 || selectedCards.Contains(_card))
			{
				_card.IsSelected = !_card.IsSelected;
			}
		}
	}

	private void OnHiglightEvent(object sender, bool highlighted)
	{
		if (!_card.IsTweening)
		{
			var pos = transform.position;

			if (highlighted)
			{
				StartCoroutine(MoveTo(new Vector3(pos.x, _maxY, pos.z), .1f));
			}
			else
			{
				StartCoroutine(MoveTo(new Vector3(pos.x, _minY, pos.z), .1f));
			}
		}
	}

	public IEnumerator MoveTo(Vector3 end, float moveDuration, Action function = null)
	{
		float t = 0.0f;

		while (t < moveDuration)
		{
			t += Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, end, t / moveDuration);

			if (Vector3.Distance(transform.position, end) < 0.001f)
			{
				transform.position = end;
				function?.Invoke();
				yield break;
			}

			yield return null;
		}
	}

	public IEnumerator RotateTo(Vector3 end, float rotateDuration, Action function = null)
	{

		float t = 0.0f;

		while (t < rotateDuration)
		{
			t += Time.deltaTime;
			var rot = transform.rotation;
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(end), t / rotateDuration);

			if (Vector3.Distance(transform.rotation.eulerAngles, Quaternion.Euler(end).eulerAngles) < 0.01f)
			{
				transform.rotation = Quaternion.Euler(end);
				function?.Invoke();
				yield break;
			}

			yield return null;
		}
	}
}
