# ML AGENTS TUTORIAL: PYTHON WORKFLOW

A quick introduction to the Python workflow to use Unity's ML Agents 3.0.0, by David de Miguel.

These steps are based on ML-Agents Release 22. They arerelevant in February 2025. Always check for updates on this [Github link](https://github.com/Unity-Technologies/ml-agents) for the latest documentation.

## Python Workflow
Make sure you have downloaded the correct version of Python. The ML Agents documentation recommends **3.10.12**, however, it is not available and I am using **3.10.09**.

### Follow these steps:

#### 1. C++ Redistributable

Install latest version of C++ Redistributable

#### 2. CUDA

You can also install CUDA if your GPU is compatible. You can check in this [Nvidia CUDA GPUs Link](https://developer.nvidia.com/cuda-gpus) your GPU compatibility. If your GPU scores 6 or higher, then you should install CUDA.
> Do not install the latest version but the one compatible with the version of **pytorch**
> For **pytorch 2.2.1** is **cu121** or also called **CUDA 12.1**

#### 3. CuDNN

Optionally, you can install CuDNN to optimise the GPU for deep learning and deep neural networks.

#### 4. Create Virtual Env

##### 4.1 Open the command

Open the command, pressing Windows Key and typing: **"cmd"**

##### 4.2 Change directory

Change directory ("cd") to your Unity Project, for instance:

```sh \n
cd C:\Unity_Projects\MLA_AC_25
```

##### 4.3 Create virtual environment

```sh \n
python -m venv venv
```

##### 4.4 Activate the virtual env 

```sh \n
venv\Scripts\activate
```

Later on, to deactivate, just enter deactivate:

```sh \n
deactivate
```

#### 5 Update pip

```sh \n
python -m pip install --upgrade pip
```

#### 6 Install Numpy and Pytorch

Install ***Numpy*** (correct version) and ***Pytorch***, usually it works to request a ***numpy*** version below 2. For that, use the code below:  
```sh
pip install "numpy<2"
pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
```
#### 7. Install mlagents package

Install the ***ML Agents*** Python Package (it might take some time):

```sh
pip install mlagents
```

If you have some incompatibility errors check this video from [Code Monkey](https://www.youtube.com/watch?v=zPFU30tbyKs&list=PLzDRvYVwl53vehwiN_odYJkPBzcqFw110) (min 6:10).

#### 8. Test the installation:

```sh
mlagents-learn --help
```

### Useful mlagents commands

These are some of the key actions to be performed from the mlagents python package.

|Options|Description|
|---|---|
|-h, --help| shows the help message|
|--env ENV_PATH |Path to the Unity executable to train (default: None)|
|--resume |Whether to resume training from a checkpoint. Specify a --run-id to use this option. If set, the training code loads an already trained model to initialize the neural network before resuming training. This option is only valid when the models exist, and have the same behavior names as the current agents in your scene. (default: False)|
|--force|Whether to force-overwrite this run-id's existing summary and model data. Without this flag, attempting to train a model with a run-id that has been used before will throw an error. (default: False)|
|--initialize-from RUN_ID| Specify a previously saved run ID from which to initialize the model from. This can be used, for instance, to fine-tune an existing model on a new environment. Note that the previously saved models must have the same behavior parameters as your current environment. (default: None)|
|--seed SEED | A number to use as a seed for the random number generator used by the training code (default: -1)|
|--inference |Whether to run in Python inference mode (i.e. no training). Use with --resume to load a model trained with an existing run ID. (default: False)|
|--num-envs NUM_ENVS |The number of concurrent Unity environment instances to collect experiences from when training (default: 1)|
|--num-areas NUM_AREAS|The number of parallel training areas in each Unity environment instance. (default: 1)|
