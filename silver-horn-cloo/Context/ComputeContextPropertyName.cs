namespace SilverHorn.Cloo.Context
{
    /// <summary>
    /// 
    /// </summary>
    public enum ComputeContextPropertyName : int
    {
        /// <summary> </summary>
        Platform = ComputeContextInfo.Platform,
        /// <summary> </summary>
        CL_GL_CONTEXT_KHR = 0x2008,
        /// <summary> </summary>
        CL_EGL_DISPLAY_KHR = 0x2009,
        /// <summary> </summary>
        CL_GLX_DISPLAY_KHR = 0x200A,
        /// <summary> </summary>
        CL_WGL_HDC_KHR = 0x200B,
        /// <summary> </summary>
        CL_CGL_SHAREGROUP_KHR = 0x200C,
    }
}
