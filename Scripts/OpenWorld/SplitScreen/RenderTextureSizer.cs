using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureSizer : MonoBehaviour
{
    RectTransform m_rect;
    RawImage m_image;
    [SerializeField] bool alignUpperEdge;
    [SerializeField] RenderTexture rtex;
    void Start()
    {
        m_rect = GetComponent<RectTransform>();
        if (alignUpperEdge) upAlignResize();
        else downAlignResize();

        m_image = GetComponent<RawImage>();

        rtex.width = (int)(m_rect.rect.width);
        rtex.height = (int)m_rect.rect.height;
        m_image.texture = rtex;
    }

    void upAlignResize()
    {
        m_rect.position = new Vector3(
            m_rect.position.x,
            Screen.height * 3/5,
            m_rect.position.z);
        m_rect.sizeDelta = new Vector2(m_rect.sizeDelta.x, Screen.height *2/5);
    }

    void downAlignResize()
    {
        m_rect.position = new Vector3(
            m_rect.position.x,
            Screen.height / 5,
            m_rect.position.z);
        m_rect.sizeDelta = new Vector2(m_rect.sizeDelta.x, Screen.height *2/5);
    }
}
