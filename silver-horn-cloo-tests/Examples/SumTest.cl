__kernel void floatVectorSum(__global float* v1, __global float* v2)
{
  int i = get_global_id(0);
  v1[i] = v1[i] + v2[i];
}

__kernel void doubleVectorSum(__global double* v1, __global double* v2)
{
  int i = get_global_id(0);
  v1[i] = v1[i] + v2[i];
}
