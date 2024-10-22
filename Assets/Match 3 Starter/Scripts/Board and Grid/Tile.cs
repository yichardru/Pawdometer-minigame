﻿/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	[SerializeField]

	private float colliderSize = 1;

	void Awake() {
		render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

	private void OnMouseDown()
	{
		if (render.sprite == null || BoardManager.instance.IsShifting || EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		if (isSelected)
		{
			Deselect();
		}
		else
		{ 
			if(previousSelected == null)
            {
				Select();
            }
            else
            {
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
				{
					SwapSprite(previousSelected.render);
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
				}
				else
				{
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}

			}
		}
	}

	public void SwapSprite(SpriteRenderer render2)
    {
		if(render.sprite == render2.sprite || render.sprite == null)
        {
			return;
        }

		Sprite tempSprite = render2.sprite;
		render2.sprite = render.sprite;
		render.sprite = tempSprite;
		SFXManager.instance.PlaySFX(Clip.Swap);
		GUIManager.instance.MoveCounter--;
	}

	private GameObject GetAdjacent(Vector2 castDir)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir, 4);
		foreach (var h in Physics2D.RaycastAll(transform.position, castDir, colliderSize))
		{
			if (h.collider != GetComponent<Collider2D>())
			{
				hit = h;
			}
		}
		if (hit.collider != null && hit.collider.gameObject != this.gameObject)
		{
			return hit.collider.gameObject;
		}
		return null;
	}

	private List<GameObject> GetAllAdjacentTiles()
	{
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
		{
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return adjacentTiles;
	}

	private List<GameObject> FindMatch(Vector2 castDir)
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		GameObject hitG = GetAdjacent(castDir);
		for(int i = 0; i < 4; i++)
        {
			if(hitG != null && hitG?.GetComponent<SpriteRenderer>().sprite == render.sprite && !matchingTiles.Contains(hitG))
            {
				matchingTiles.Add(hitG);
				hitG = hitG.GetComponent<Tile>().GetAdjacent(castDir);
            }
			if (hitG == null || hitG?.GetComponent<SpriteRenderer>().sprite != render.sprite || matchingTiles.Contains(hitG))
            {
				break;
            }
        }
		return matchingTiles;
	}

	private void ClearMatch(Vector2[] paths)
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		for (int i = 0; i < paths.Length; i++)
		{
			//Debug.LogError("Entering Find Match");
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2)
		{
			for (int i = 0; i < matchingTiles.Count; i++)
			{
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			}
			matchFound = true;
		}
	}

	private bool matchFound = false;
	public void ClearAllMatches()
	{
		if (render.sprite == null)
			return;

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
		{
			render.sprite = null;
			matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			SFXManager.instance.PlaySFX(Clip.Clear);
			
		}
	}

	/**
	private List<GameObject> FindMatch (Vector2 castDir)
    {
		List<GameObject> matchingTiles = new List<GameObject>();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
		return matchingTiles;
    }
	

	private void ClearMatch(Vector2[] paths)
    {
		List<GameObject> matchingTiles = new List<GameObject>();
		for(int i = 0; i < matchingTiles.Count; i++)
        {
			matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
        }
		matchFound = true;
    }

	private void ClearAllMatches()
    {
		if(render.sprite == null)
        {
			return;
        }
		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (matchFound)
        {
			render.sprite = null;
			matchFound = false;
			SFXManager.instance.PlaySFX(Clip.Clear);
        }
	}
	**/
}