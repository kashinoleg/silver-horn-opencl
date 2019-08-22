#include "pch.h"
//#define CL_TARGET_OPENCL_VERSION 100
#define CL_TARGET_OPENCL_VERSION 100
#include "CL/opencl.h"
#include <cstdio>
#include <crtdbg.h>

int a()
{
	cl_program program = NULL;
	cl_kernel kernel = NULL;
	cl_device_id device_id = NULL;
	cl_context context = NULL;
	cl_int ret = 0;

	FILE* fp;
	const char fileName[] = "../forTest.cl";
	size_t source_size;
	char* source_str;
	int i;
	int MAX_SOURCE_SIZE = 200;

	try {
		fp = fopen(fileName, "r");
		if (!fp) {
			fprintf(stderr, "Failed to load kernel.\n");
			//exit(1);
		}
		//source_str = (char*)malloc(MAX_SOURCE_SIZE);
		source_size = fread(source_str, 1, MAX_SOURCE_SIZE, fp);
		fclose(fp);
	}
	catch (int a) {
		printf("%f", a);
	}

	/* создать бинарник из кода программы */
	program = clCreateProgramWithSource(context, 1, (const char**)& source_str, (const size_t*)& source_size, &ret);

	/* скомпилировать программу */
	ret = clBuildProgram(program, 1, &device_id, NULL, NULL, NULL);

	/* создать кернел */
	kernel = clCreateKernel(program, "test", &ret);
}