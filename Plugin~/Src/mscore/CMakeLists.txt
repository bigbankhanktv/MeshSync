CMAKE_MINIMUM_REQUIRED(VERSION 3.5)

project(mscore)
file(GLOB_RECURSE sources *.cpp *.h)
set(mscore_dir "${CMAKE_CURRENT_SOURCE_DIR}")
set(SRC_ROOT "${CMAKE_CURRENT_SOURCE_DIR}/..")
set(plugins_dir "${SRC_ROOT}/../../Runtime/Plugins/x86_64")

add_plugin(mscore SOURCES ${sources} PLUGINS_DIR ${plugins_dir})
source_group(TREE ${CMAKE_CURRENT_SOURCE_DIR} FILES ${sources})

#Include, pch
target_include_directories(mscore PRIVATE 
    ${mscore_dir}
)
target_include_directories(mscore PRIVATE )
target_precompile_headers(mscore PRIVATE "${mscore_dir}/pch.h")

# Library dependencies
target_link_libraries( mscore 
    PUBLIC 
        MeshSync 
)

